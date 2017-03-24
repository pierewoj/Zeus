using System;
using System.Collections.Generic;
using System.Text;

namespace Zeus.Crawler
{
    interface IShouldCrawlDecider
    {
        bool ShouldCrawl(CrawlablePage page);
    }

    class ShouldCrawlDecider : IShouldCrawlDecider
    {
        public bool ShouldCrawl(CrawlablePage page)
        {
            return page.Uri.Host.Contains("kwestiasmaku.com");
        }
    }
}
