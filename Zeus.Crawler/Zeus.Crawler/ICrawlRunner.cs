using System.Collections.Generic;
using System.Linq;
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
        private readonly IShouldCrawlPagePredicate _shouldCrawlPagePredicate;
        private readonly ICrawler _crawler;
        private readonly ILogger _logger;

        public CrawlRunner(IPageCrawlResultSaver saver, IResultSavedNotifier notifier, IShouldCrawlPagePredicate shouldCrawlPredicate,
            ICrawler crawler, ILogger<CrawlRunner> logger)
        {
            _logger = logger;
            _crawler = crawler;
            _shouldCrawlPagePredicate = shouldCrawlPredicate;
            _resultSavedNotifier = notifier;
            _pageCrawlResultSaver = saver;
        }

        public void Run(CrawlablePage startPage)
        {
            var pagesToBeCrawled = new Queue<CrawlablePage>();
            pagesToBeCrawled.Enqueue(startPage);

            _logger.LogInformation("Crawl run starting.");
            while (pagesToBeCrawled.Any())
            {
                var page = pagesToBeCrawled.Dequeue();
                if (!_shouldCrawlPagePredicate.ShouldCrawl(page))
                    continue;
                var res = _crawler.Crawl(page);
                pagesToBeCrawled.Enqueue(res.CrawlablePages.Where(_shouldCrawlPagePredicate.ShouldCrawl));
                var savingResult = _pageCrawlResultSaver.SaveResult(res);
                _resultSavedNotifier.Notify(savingResult);
            }
            _logger.LogInformation("No more pages to crawl. Exiting crawl run.");
        }
    }
}
