using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt;

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
        private readonly HashSet<CrawlablePage> _savedPages = new HashSet<CrawlablePage>();
        private readonly IShouldCrawlDecider _shouldCrawlDecider;

        public CrawlablePagesRepository(IShouldCrawlDecider shouldCrawlDecider)
        {
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
                 return Option<CrawlablePage>.None;

            var page = _crawlablePagesQueue.Dequeue();
            return page;
        }

        public void Save(CrawlablePage page)
        {
            if (!_savedPages.Contains(page) && _shouldCrawlDecider.ShouldCrawl(page))
            {
                _savedPages.Add(page);
                _crawlablePagesQueue.Enqueue(page);
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
