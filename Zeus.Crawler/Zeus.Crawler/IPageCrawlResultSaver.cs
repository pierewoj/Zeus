using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Zeus.Crawler
{
    interface IPageCrawlResultSaver
    {
        SavingResult SaveResult(PageCrawlResult result);
    }

    class PageCrawlResultSaver : IPageCrawlResultSaver
    {
        private readonly ILogger<PageCrawlResultSaver> _logger;

        public PageCrawlResultSaver(ILogger<PageCrawlResultSaver> logger)
        {
            _logger = logger;
        }
        
        public SavingResult SaveResult(PageCrawlResult result)
        {
            while (true)
            {
                try
                {
                    PushToRabbit(result);
                    return new SavingResult();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Failed to push to rabbit. Retrying.");
                    Thread.Sleep(1000);
                }
            }
        }

        private void PushToRabbit(PageCrawlResult result)
        {
            var exchangeName = "kwestiasmaku_crawled";
            var rabbitHost = "rabbit";

            var factory = new ConnectionFactory() { HostName = rabbitHost };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");

                var message = new
                {
                    Html = result.Html,
                    Uri = result.Url
                };
                var messageSerialized = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(messageSerialized);
                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
                _logger.LogInformation($"Published page crawl result of uri [{result.Url}] to exchange [{exchangeName}] on host [{rabbitHost}]");
            }

        }
    }
}
