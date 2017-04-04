using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Zeus.Crawler
{
    interface ICrawlResultSaver
    {
        void Save(PageCrawlResult page);
    }

    class CrawlResultSaver : ICrawlResultSaver
    {
        private readonly ILogger<CrawlResultSaver> _logger;
        private readonly IRedisProvider _redisProvider;

        public CrawlResultSaver(ILogger<CrawlResultSaver> logger,IRedisProvider redisProvider)
        {
            _redisProvider = redisProvider;
            _logger = logger;
        }

        public void Save(PageCrawlResult page)
        {
            _logger.LogInformation($"Saving crawl result of page [{page.Url}] to the database.");
            var db = _redisProvider.GetDatabase();
            var serialized = JsonConvert.SerializeObject(page);
            db.StringSet(page.Url, serialized);
        }
    }
}
