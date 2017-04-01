using System.Collections.Generic;

namespace Zeus.Crawler.Models
{
    public class CrawledPageCollectionModel
    {
        public IEnumerable<CrawledPageModel> CrawlablePages { get; set; }
    }
}