using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Zeus.Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection().AddLogging();
            services.AddSingleton<ICrawlRunner, CrawlRunner>();
            services.AddSingleton<ICrawler, Crawler>();
            services.AddSingleton<IPageCrawledNotifier, PageCrawledNotifier>();
            services.AddSingleton<ICrawlResultSaver, CrawlResultSaver>();
            services.AddSingleton<ICrawlablePagesRepository, CrawlablePagesRepository>();
            services.AddSingleton<ILinksExtractor, LinksExtractor>();
            services.AddSingleton<IQueryProcessor, QueryProcessor>();
            services.AddSingleton<IRedisProvider, RedisProvider>();
            var serviceProvider = services.BuildServiceProvider();

            //configuring serilog
            var log = new LoggerConfiguration()
               .WriteTo.RollingFile("log-{Date}.txt")
               .WriteTo.LiterateConsole()
               .CreateLogger();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddSerilog(log);

            var runner = serviceProvider.GetService<ICrawlRunner>();
            
            runner.Run();
        }
    }
}