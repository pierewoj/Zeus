using System.Collections.Generic;

namespace Zeus.Crawler
{
    class PageCrawlResult
    {
        public IEnumerable<CrawlablePage> CrawlablePages { get; set; } = new List<CrawlablePage>();
    }
}
