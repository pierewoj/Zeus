using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Shouldly;
using Xunit;

namespace Zeus.Crawler.Tests
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public class LinksExtractorTests
    {
        private readonly LinksExtractor _extractor = new LinksExtractor();

        [Fact]
        public void Extracts_WhenWhenAbsoluteLink()
        {
            var simpleLink = @"<a href=""www.wp.pl"">ddd</a>";
            var results = _extractor.ExtractLinks(simpleLink);
            results.Count().ShouldBe(1);
            results.First().ShouldBe("www.wp.pl");
        }

        [Fact]
        public void Extracts_WhenRelativeLink()
        {
            var simpleLink = @"<a href=""/test"">ddd</a>";
            var results = _extractor.ExtractLinks(simpleLink);
            results.Count().ShouldBe(1);
            results.First().ShouldBe("/test");
        }

        [Fact]
        public void Extracts_WhenLinkWithHttp()
        {
            var simpleLink = @"<a href=""http://wp.pl"">ddd</a>";
            var results = _extractor.ExtractLinks(simpleLink);
            results.Count().ShouldBe(1);
            results.First().ShouldBe("http://wp.pl");
        }

        [Fact]
        public void Extracts_WhenTwoLinks()
        {
            var simpleLink = @"<a href=""http://wp.pl"">ddd</a>"+
                @"<a href=""http://o2.pl"">ddd</a>";
            var results = _extractor.ExtractLinks(simpleLink);
            results.Count().ShouldBe(2);
            results.ShouldContain("http://wp.pl");
            results.ShouldContain("http://o2.pl");
        }

        [Fact]
        public void Extracs_WhenOtherTagsThanHref()
        {
            var simpleLink = @"<a href=""http://wp.pl"">ddd</a>";
            var results = _extractor.ExtractLinks(simpleLink);
            results.Count().ShouldBe(1);
            results.First().ShouldBe("http://wp.pl");
        }
    }
}
