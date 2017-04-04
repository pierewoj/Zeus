using Microsoft.Extensions.Logging;

namespace Zeus.Crawler
{
    interface ICrawlRunner
    {
        void Run();
    }

    class CrawlRunner : ICrawlRunner
    {
        private readonly IPageCrawledNotifier _pageCrawledNotifier;
        private readonly ICrawlResultSaver _crawlResultSaver;
        private readonly ICrawler _crawler;
        private readonly ILogger _logger;
        private readonly ICrawlablePagesRepository _crawlablePagesRepository;

        public CrawlRunner(IPageCrawledNotifier saver, ICrawlResultSaver notifier, ICrawlablePagesRepository pagesRepository,
            ICrawler crawler, ILogger<CrawlRunner> logger)
        {
            _crawlablePagesRepository = pagesRepository;
            _logger = logger;
            _crawler = crawler;
            _crawlResultSaver = notifier;
            _pageCrawledNotifier = saver;
        }

        public void Run()
        {
            _logger.LogInformation("Crawl run starting.");
            var pageOption = _crawlablePagesRepository.GetPage();
            do
            {
                pageOption.IfSome(x => Crawl(x));
                pageOption = _crawlablePagesRepository.GetPage();
            } while (pageOption.IsSome);
            
            _logger.LogInformation("No more pages to crawl. Exiting crawl run.");
        }

        private void Crawl(CrawlablePage page)
        {
            var resOption = _crawler.Crawl(page);
            resOption.IfSome(res =>
            {
                _pageCrawledNotifier.Notify(res);
                _crawlResultSaver.Save(res);
                _crawlablePagesRepository.Delete(res.Url);
            });
        }
    }
}
