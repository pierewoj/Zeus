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
            var stringed = page.Uri.ToString();
            return
                stringed.StartsWith("http://kwestiasmaku.com")
                && !stringed.Contains("taxonomy")
                && !stringed.Contains("sort_by")
                && !stringed.Contains("login")
                && !stringed.Contains("user");
        }
    }
}
