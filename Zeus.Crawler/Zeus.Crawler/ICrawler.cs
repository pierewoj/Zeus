using System;
using System.Globalization;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Zeus.Crawler
{
    interface ICrawler
    {
        Option<PageCrawlResult> Crawl(CrawlablePage page);
    }

    class Crawler : ICrawler
    {
        private readonly ILogger<ICrawler> _logger;
        private readonly ILinksExtractor _linksExtractor;
        private readonly IQueryProcessor _queryProcessor;

        public Crawler(ILogger<ICrawler> logger, ILinksExtractor extractor, IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
            _linksExtractor = extractor;
            _logger = logger;
        }
        public Option<PageCrawlResult> Crawl(CrawlablePage page)
        {
            _logger.LogInformation($"Crawling page {page.Uri}");
            try
            {
                var uri = new Uri(page.Uri);
                var contentOption = _queryProcessor.ProcessQuery(uri);
                return contentOption.Match(content =>
                {
                    var links = _linksExtractor.ExtractLinks(content);
                    return new PageCrawlResult()
                    {
                        Html = content,
                        Url = page.Uri.ToString(),
                        TimeCrawled = DateTime.Now.ToString(CultureInfo.InvariantCulture)
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
