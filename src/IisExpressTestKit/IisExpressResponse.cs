using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;

using Xunit;

namespace IisExpressTestKit
{
    public class IisExpressResponse
    {
        public string Host { get; set; }
        public string Path { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public NameValueCollection Headers { get; set; }
        public string Body { get; set; }

        public IisExpressResponse IsPath(string expectedPath)
        {
            Assert.Equal(expectedPath, Path);

            return this;
        }

        public IisExpressResponse IsRedirect(string expectedUrl)
        {
            Assert.Equal(expectedUrl, Headers["Location"]);

            return this;
        }

        public IisExpressResponse IsStatusCode(HttpStatusCode expectedStatusCode)
        {
            Assert.Equal(expectedStatusCode, StatusCode);

            return this;
        }

        public IisExpressResponse IsHeaderValue(string headerName, string expectedValue)
        {
            Assert.Equal(expectedValue, Headers[headerName]);

            return this;
        }

        public IisExpressResponse Contains(string expectedSubstring)
        {
            Assert.Contains(expectedSubstring, Body);

            return this;
        }

        public IisExpressResponse DoesNotContain(string expectedSubstring)
        {
            Assert.DoesNotContain(expectedSubstring, Body);

            return this;
        }

        public IisExpressResponse HtmlAttribute(string tagName, string attributeName, string expectedValue)
        {
            var tagMatch = Regex.Match(Body, $"<{tagName}.*?>");

            Assert.True(tagMatch.Success);

            var content = tagMatch.Value;

            var attributeMatch = Regex.Match(content, $"{Regex.Escape(attributeName)}=\"?([^\"]*)\"?");

            Assert.True(attributeMatch.Success);

            Assert.Contains(expectedValue, attributeMatch.Groups[1].Value);

            return this;
        }
    }
}
