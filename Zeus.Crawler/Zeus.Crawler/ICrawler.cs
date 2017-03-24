using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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
        private readonly ICrawlablePageBuilder _crawlablePageBuilder;
        private readonly ILinksExtractor _linksExtractor;

        public Crawler(ILogger<ICrawler> logger, ILinksExtractor extractor, ICrawlablePageBuilder crawlablePageBuilder)
        {
            _linksExtractor = extractor;
            _crawlablePageBuilder = crawlablePageBuilder;
            _logger = logger;
        }
        public Option<PageCrawlResult> Crawl(CrawlablePage page)
        {
            _logger.LogInformation($"Crawling page {page.Uri}");
            try
            {
                var httpClient = new HttpClient();
                var response = httpClient.GetAsync(page.Uri);
                response.Result.EnsureSuccessStatusCode();
                var content = response.Result.Content.ReadAsStringAsync().Result;
                var links = _linksExtractor.ExtractLinks(content);
                var crawlablePages = links.Select(_crawlablePageBuilder.Build);
                return new PageCrawlResult()
                {
                    CrawlablePages = crawlablePages
                };

            }
            catch(Exception ex)
            {
                _logger.LogError(0, ex, $"Exception when crawling page {page.Uri}");
                return Option<PageCrawlResult>.None;
            }
        }
    }
}
