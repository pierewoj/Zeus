using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zeus.Crawler.Models;

namespace Zeus.Crawler
{
    interface ICrawlablePagesRepository
    {
        Option<CrawlablePage> GetPage();
        void Delete(string uri);
    }

    class CrawlablePagesRepository : ICrawlablePagesRepository
    {
        private readonly ILogger<CrawlablePagesRepository> _logger;

        public CrawlablePagesRepository(ILogger<CrawlablePagesRepository> logger)
        {
            _logger = logger;
        }

        public Option<CrawlablePage> GetPage()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestUri = new Uri("http://athena/pages/crawlable/random");
                    var response = client.GetAsync(requestUri).Result;
                    response.EnsureSuccessStatusCode();
                    var content = response.Content.ReadAsStringAsync().Result;
                    var page = JsonConvert.DeserializeObject<CrawlablePage>(content);
                    return page;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(0, ex, $"Failed to get page to crawl.");
                return null;
            }
            
        }

        public void Delete(string uri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestUri = new Uri("http://athena/pages/crawlable");
                    var model = new CrawledPageModel()
                    {
                        Uri = uri
                    };
                    var serializedModel = JsonConvert.SerializeObject(model);
                    var content = new StringContent(serializedModel, Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage()
                    {
                        Content = content,
                        Method = HttpMethod.Delete,
                        RequestUri = requestUri
                    };
                    var res = client.SendAsync(request).Result;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, $"Failed ot delete crawled page [{uri}]");
            }
            
        }
    }
}
