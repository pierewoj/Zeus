using Microsoft.Extensions.Logging;

namespace Zeus.Crawler
{
    interface IResultSavedNotifier
    {
        void Notify(PageCrawlResult savingResult);
    }

    class ResultSavedNotifier : IResultSavedNotifier
    {
        private readonly ICrawlablePagesRepository _crawlablePagesRepository;
        private readonly ILogger<ResultSavedNotifier> _logger;

        public ResultSavedNotifier(ILogger<ResultSavedNotifier> logger, ICrawlablePagesRepository repository)
        {
            _logger = logger;
            _crawlablePagesRepository = repository;
        }

        public void Notify(PageCrawlResult savingResult)
        {
            _crawlablePagesRepository.Delete(savingResult.Url);
        }
    }
}
