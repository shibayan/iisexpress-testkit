using System;
using System.IO;
using System.Web;

namespace IisExpressTestKit
{
    public class EchoHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
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
                    context.Response.Headers["Content-Type"] = MimeMapping.GetMimeMapping(originalFile);
                    context.Response.WriteFile(originalFile);
                }

                context.Response.StatusCode = 200;
            }
        }

        public bool IsReusable => true;
    }
}