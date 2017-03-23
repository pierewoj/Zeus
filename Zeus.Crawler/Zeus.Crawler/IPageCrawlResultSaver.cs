namespace Zeus.Crawler
{
    interface IPageCrawlResultSaver
    {
        SavingResult SaveResult(PageCrawlResult result);
    }

    class PageCrawlResultSaver : IPageCrawlResultSaver
    {
        public SavingResult SaveResult(PageCrawlResult result)
        {
            return new SavingResult();
        }
    }
}
