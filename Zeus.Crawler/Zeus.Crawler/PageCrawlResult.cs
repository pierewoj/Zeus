using System.Collections.Generic;

namespace Zeus.Crawler
{
    class PageCrawlResult
    {
        public string Html { get; set; }
        public string Url { get; set; }
        public IEnumerable<CrawlablePage> CrawlablePages { get; set; } = new List<CrawlablePage>();
    }
}
