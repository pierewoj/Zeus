using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using Consul.SimpleDiscovery;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zeus.Crawler.Models;

namespace Zeus.Crawler
{
    interface ICrawlablePagesRepository
    {
        Option<CrawlablePage> GetPage();
        void Save(IEnumerable<CrawlablePage> pages);
        void Delete(string uri);
    }

    class CrawlablePagesRepository : ICrawlablePagesRepository
    {
        private readonly ILogger<CrawlablePagesRepository> _logger;
        private readonly IServiceResolver _serviceResolver;

        public CrawlablePagesRepository(ILogger<CrawlablePagesRepository> logger, IServiceResolver resolver)
        {
            _serviceResolver = resolver;
            _logger = logger;
        }

        public Option<CrawlablePage> GetPage()
        {
            using(var client = new HttpClient())
            {
                var requestUri = new Uri("http://athena/pages/crawlable/random");
                var response = client.GetAsync(requestUri).Result;
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;
                var page = JsonConvert.DeserializeObject<CrawlablePage>(content);
                return page;
            }
        }

        public void Save(IEnumerable<CrawlablePage> pages)
        {
            using (var client = new HttpClient())
            {
                var requestUri = new Uri("http://athena/pages/crawlable");
                var model = new CrawledPageCollectionModel()
                {
                    CrawlablePages = pages.Select(x => new CrawledPageModel()
                    {
                        Uri = x.Uri.ToString()
                    })
                };
                var serializedModel = JsonConvert.SerializeObject(model);
                var content = new StringContent(serializedModel, Encoding.UTF8, "application/json");
                var res = client.PutAsync(requestUri, content).Result;
            }
        }

        public void Delete(string uri)
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
    }
}
