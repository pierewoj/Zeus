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
        private readonly Queue<CrawlablePage> _crawlablePages = new Queue<CrawlablePage>();

        public CrawlablePagesRepository()
        {
            var startPage = new CrawlablePage()
            {
                Uri = new Uri("http://kwestiasmaku.com")
            };
            _crawlablePages.Enqueue(startPage);
        }

        public Option<CrawlablePage> GetPage()
        {
            if (_crawlablePages.Any())
            {
                var page = _crawlablePages.Dequeue();
                return Option<CrawlablePage>.Some(page);
            }
            return Option<CrawlablePage>.None;
        }

        public void Save(CrawlablePage page)
        {
            _crawlablePages.Enqueue(page);
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
