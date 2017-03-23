using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

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
                pageOption.Match(Some:Crawl, None:() => {});
                pageOption = _crawlablePagesRepository.GetPage();
            } while (pageOption.IsSome);
            
            _logger.LogInformation("No more pages to crawl. Exiting crawl run.");
        }

        private void Crawl(CrawlablePage page)
        {
            var res = _crawler.Crawl(page);
            _crawlablePagesRepository.Save(res.CrawlablePages);
            var savingResult = _pageCrawlResultSaver.SaveResult(res);
            _resultSavedNotifier.Notify(savingResult);
        }
    }
}
