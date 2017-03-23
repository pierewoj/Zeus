namespace Zeus.Crawler
{
    interface IResultSavedNotifier
    {
        void Notify(SavingResult savingResult);
    }

    class ResultSavedNotifier : IResultSavedNotifier
    {
        public void Notify(SavingResult savingResult)
        {
        }
    }
}
