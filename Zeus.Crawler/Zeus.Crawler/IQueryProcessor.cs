using System;
using System.Net.Http;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Zeus.Crawler
{
    public interface IQueryProcessor
    {
        Option<string> ProcessQuery(Uri uri);
    }

    class QueryProcessor : IQueryProcessor
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly ILogger _logger;

        public QueryProcessor(ILogger<QueryProcessor> logger)
        {
            _logger = logger;
        }

        public Option<string> ProcessQuery(Uri uri)
        {
            try
            {
                _logger.LogInformation($"Making GET on [{uri}]");
                var response = _client.GetAsync(uri).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch(Exception ex)
            {
                _logger.LogError(0, ex, "Exception when processing http query");
                return Option<string>.None;
            }
        }
    }
}
