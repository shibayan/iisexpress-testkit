using System;
using System.IO;
using System.Linq;
using System.Web;

namespace IisExpressTestKit
{
    public class EchoHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.TrySkipIisCustomErrors = true;

            if (File.Exists(context.Request.PhysicalPath) || Directory.Exists(context.Request.PhysicalPath))
            {
                var type = typeof(HttpApplication).Assembly.GetType("System.Web.StaticFileHandler", true);
                var handler = (IHttpHandler)Activator.CreateInstance(type, true);
                
                handler.ProcessRequest(context);
            }
            else
            {
                var originalFile = context.Request.Headers["X-OriginalFile"];

                if (!string.IsNullOrEmpty(originalFile))
                {
                    context.Response.ContentType = MimeMapping.GetMimeMapping(originalFile);
                    context.Response.WriteFile(originalFile);
                }

                int statusCode;

                context.Response.StatusCode = int.TryParse(context.Request.Headers["X-Iis-StatusCode"], out statusCode) ? statusCode : 200;

                foreach (var key in context.Request.Headers.AllKeys.Where(x => x.StartsWith("X-Iis-Header-")))
                {
                    var headerKey = key.Substring(13);

                    if (headerKey == "Content-Type")
                    {
                        context.Response.ContentType = context.Request.Headers[key];
                    }
                    else
                    {
                        context.Response.Headers[headerKey] = context.Request.Headers[key];
                    }
                }
            }
        }

        public bool IsReusable => true;
    }
}