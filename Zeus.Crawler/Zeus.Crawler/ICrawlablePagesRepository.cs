using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Zeus.Crawler
{
    interface ICrawlablePagesRepository
    {
        Option<CrawlablePage> GetPage();
        void Save(CrawlablePage page);
        void Save(IEnumerable<CrawlablePage> pages);
    }

    class CrawlablePagesRepository : ICrawlablePagesRepository
    {
        private readonly Queue<CrawlablePage> _crawlablePagesQueue = new Queue<CrawlablePage>();
        private readonly HashSet<string> _savedPages = new HashSet<string>();
        private readonly IShouldCrawlDecider _shouldCrawlDecider;
        private readonly ILogger<CrawlablePagesRepository> _logger;

        public CrawlablePagesRepository(IShouldCrawlDecider shouldCrawlDecider, ILogger<CrawlablePagesRepository> logger)
        {
            _logger = logger;
            _shouldCrawlDecider = shouldCrawlDecider;
            var startPage = new CrawlablePage()
            {
                Uri = new Uri(Configuration.BaseUrl)
            };
            _crawlablePagesQueue.Enqueue(startPage);
        }

        public Option<CrawlablePage> GetPage()
        {
            if (!_crawlablePagesQueue.Any())
            {
                _logger.LogInformation("No more pages to crawl. Returning None.");
                 return Option<CrawlablePage>.None;
            }

            var page = _crawlablePagesQueue.Dequeue();
            return page;
        }

        public void Save(CrawlablePage page)
        {
            var pageSavedBefore = _savedPages.Contains(page.Uri.ToString());
            var shouldCrawlPage = _shouldCrawlDecider.ShouldCrawl(page);

            if (!pageSavedBefore && shouldCrawlPage)
            {
                _logger.LogDebug($"Saving page [{page.Uri}] and adding it to the queue. SavedCount={_savedPages.Count}");
                _savedPages.Add(page.Uri.ToString());
                _crawlablePagesQueue.Enqueue(page);
            }
            else
            {
                _logger.LogDebug($"Ignoring saving page [{page.Uri}] and adding it to the queue. PageSavedBefore={pageSavedBefore}, ShouldCrawl={shouldCrawlPage}");
            }
        }

        public void Save(IEnumerable<CrawlablePage> pages)
        {
            foreach (var crawlablePage in pages)
            {
                Save(crawlablePage);
            }
        }
    }
}
