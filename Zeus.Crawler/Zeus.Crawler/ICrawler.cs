using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using LanguageExt;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Zeus.Crawler
{
    interface ICrawler
    {
        Option<PageCrawlResult> Crawl(CrawlablePage page);
    }

    class Crawler : ICrawler
    {
        private readonly ILogger<ICrawler> _logger;
        private readonly ICrawlablePageBuilder _crawlablePageBuilder;
        private readonly ILinksExtractor _linksExtractor;
        private readonly IQueryProcessor _queryProcessor;

        public Crawler(ILogger<ICrawler> logger, ILinksExtractor extractor, ICrawlablePageBuilder crawlablePageBuilder, IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
            _linksExtractor = extractor;
            _crawlablePageBuilder = crawlablePageBuilder;
            _logger = logger;
        }
        public Option<PageCrawlResult> Crawl(CrawlablePage page)
        {
            _logger.LogInformation($"Crawling page {page.Uri}");
            try
            {
                var contentOption = _queryProcessor.ProcessQuery(page.Uri);
                return contentOption.Match(content =>
                {
                    var links = _linksExtractor.ExtractLinks(content);
                    var crawlablePages = links.Select(_crawlablePageBuilder.Build);
                    return new PageCrawlResult()
                    {
                        Html = content,
                        Url = page.Uri.ToString(),
                        CrawlablePages = crawlablePages
                    };
                }, () => Option<PageCrawlResult>.None);
            }
            catch(Exception ex)
            {
                _logger.LogError(0, ex, $"Exception when crawling page {page.Uri}");
                return Option<PageCrawlResult>.None;
            }
        }
    }
}
