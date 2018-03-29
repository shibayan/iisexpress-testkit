using System;
using System.Web;

namespace IisExpressTestKit
{
    public class EchoHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            _context = context;

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        private HttpApplication _context;

        public void Dispose()
        {
            _context.PreSendRequestHeaders -= OnPreSendRequestHeaders;
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var context = HttpContext.Current;

            context.Response.Headers["X-IIS-RealHost"] = context.Request.Url.Host;
            context.Response.Headers["X-IIS-RealPath"] = context.Request.Url.PathAndQuery;
        }
    }
}
