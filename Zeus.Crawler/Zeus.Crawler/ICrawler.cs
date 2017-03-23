using Microsoft.Extensions.Logging;

namespace Zeus.Crawler
{
    interface ICrawler
    {
        PageCrawlResult Crawl(CrawlablePage page);
    }

    class Crawler : ICrawler
    {
        private readonly ILogger<ICrawler> _logger;

        public Crawler(ILogger<ICrawler> logger)
        {
            _logger = logger;
        }
        public PageCrawlResult Crawl(CrawlablePage page)
        {
            _logger.LogInformation($"Crawling page {page.Uri}");
            return new PageCrawlResult();
        }
    }
}
