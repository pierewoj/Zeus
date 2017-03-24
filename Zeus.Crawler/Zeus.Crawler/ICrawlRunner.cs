using Microsoft.Extensions.Logging;

namespace Zeus.Crawler
{
    interface ICrawlRunner
    {
        void Run(CrawlablePage startPage);
    }

    class CrawlRunner : ICrawlRunner
    {
        private readonly IPageCrawlResultSaver _pageCrawlResultSaver;
        private readonly IResultSavedNotifier _resultSavedNotifier;
        private readonly ICrawler _crawler;
        private readonly ILogger _logger;
        private readonly ICrawlablePagesRepository _crawlablePagesRepository;

        public CrawlRunner(IPageCrawlResultSaver saver, IResultSavedNotifier notifier, ICrawlablePagesRepository pagesRepository,
            ICrawler crawler, ILogger<CrawlRunner> logger)
        {
            _crawlablePagesRepository = pagesRepository;
            _logger = logger;
            _crawler = crawler;
            _resultSavedNotifier = notifier;
            _pageCrawlResultSaver = saver;
        }

        public void Run(CrawlablePage startPage)
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
                _crawlablePagesRepository.Save(res.CrawlablePages);
                var savingResult = _pageCrawlResultSaver.SaveResult(res);
                _resultSavedNotifier.Notify(savingResult);
            });
        }
    }
}
