using System.Web;

using Newtonsoft.Json;

namespace IisExpressTestKit
{
    public class EchoHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var data = JsonConvert.SerializeObject(new
            {
                Host = context.Request.Url.Host,
                Path = context.Request.Url.PathAndQuery
            });

            context.Response.ContentType = "application/json";
            context.Response.Write(data);
        }

        public bool IsReusable => true;
    }
}
