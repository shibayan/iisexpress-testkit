using System.Collections.Specialized;
using System.Net;

namespace IisExpressTestKit
{
    public class IisExpressRequestOptions
    {
        internal IisExpressRequestOptions() { }

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public NameValueCollection Headers { get; set; } = new NameValueCollection();
    }
}
