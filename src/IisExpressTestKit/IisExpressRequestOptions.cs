using System.Collections.Specialized;
using System.Net;

namespace IisExpressTestKit
{
    public class IisExpressRequestOptions
    {
        internal IisExpressRequestOptions()
        {
            StatusCode = HttpStatusCode.OK;
            Headers = new NameValueCollection();
        }

        public HttpStatusCode StatusCode { get; set; }

        public NameValueCollection Headers { get; set; }
    }
}
