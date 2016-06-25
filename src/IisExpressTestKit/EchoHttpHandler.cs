using System.Web;

namespace IisExpressTestKit
{
    public class EchoHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = 200;
        }

        public bool IsReusable => true;
    }
}