using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Zeus.Crawler.Models;

namespace Zeus.Crawler
{
    interface IPageCrawledNotifier
    {
        void Notify(PageCrawlResult result);
    }

    class PageCrawledNotifier : IPageCrawledNotifier
    {
        private readonly ILogger<PageCrawledNotifier> _logger;

        public PageCrawledNotifier(ILogger<PageCrawledNotifier> logger)
        {
            _logger = logger;
        }
        
        public void Notify(PageCrawlResult result)
        {
            while (true)
            {
                try
                {
                    PushToRabbit(result);
                    break;
                }
                catch (Exception)
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

                var message = new PageCrawledNotification()
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
                _logger.LogInformation($"Published notification of page crawl of uri [{result.Url}] to exchange [{exchangeName}] on host [{rabbitHost}]");
            }

        }
    }
}
