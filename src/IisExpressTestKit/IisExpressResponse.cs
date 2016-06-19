using System.Collections.Specialized;
using System.Net;

using Xunit;

namespace IisExpressTestKit
{
    public class IisExpressResponse
    {
        public string Host { get; set; }
        public string Path { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public NameValueCollection Headers { get; set; }

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
    }
}
