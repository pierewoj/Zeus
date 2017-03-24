using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zeus.Crawler
{
    public interface ILinksExtractor
    {
        IEnumerable<string> ExtractLinks(string html);
    }

    public class LinksExtractor : ILinksExtractor
    {
        public IEnumerable<string> ExtractLinks(string html)
        {
            Regex rx = new Regex(@"href="".*?""", RegexOptions.IgnoreCase);
            var matchedStrings = rx.Matches(html).Cast<Match>().Select(x => x.Value);
            var trimmed = matchedStrings.Select(Trim);
            return trimmed;
        }

        private string Trim(string link)
        {
            return link.Replace(@"href=""", "").Replace(@"""", "");
        }
    }
}
