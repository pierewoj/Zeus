using System;

namespace Zeus.Crawler
{
    public interface ICrawlablePageBuilder
    {
        CrawlablePage Build(string link);
    }

    class CrawlablePageBuilder : ICrawlablePageBuilder
    {
        public CrawlablePage Build(string link)
        {
            return new CrawlablePage()
            {
                Uri = GetUri(link)
            };
        }

        private Uri GetUri(string link)
        {
            var linkUri = new Uri(link, UriKind.RelativeOrAbsolute);
            if (linkUri.IsAbsoluteUri)
                return linkUri;
            var baseUri = new Uri(Configuration.BaseUrl);
            return new Uri(baseUri, link);
        }
    }
}
