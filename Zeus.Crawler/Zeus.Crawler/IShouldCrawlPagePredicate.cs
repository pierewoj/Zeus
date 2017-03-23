namespace Zeus.Crawler
{
    interface IShouldCrawlPagePredicate
    {
        bool ShouldCrawl(CrawlablePage page);
    }

    class ShouldCrawlPagePredicate : IShouldCrawlPagePredicate
    {
        public bool ShouldCrawl(CrawlablePage page)
        {
            return true;
        }
    }
}
