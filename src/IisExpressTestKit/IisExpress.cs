using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using Microsoft.Web.XmlTransform;

namespace IisExpressTestKit
{
    public class IisExpress : IDisposable
    {
        public IisExpress()
        {
            _port = GetPortNumber();
            _wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            ServicePointManager.Expect100Continue = false;
        }

        private readonly int _port;
        private readonly string _wwwroot;
        private Process _process;

        private static readonly Random _random = new Random();

        private const string IisExpressExe = @"IIS Express\iisexpress.exe";
        private const string WebConfigTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <system.webServer>
    <handlers>
      <add name=""EchoHttpHandler"" verb=""*"" path=""*"" type=""IisExpressTestKit.EchoHttpHandler, IisExpressTestKit"" resourceType=""Unspecified"" />
    </handlers>
    <modules runAllManagedModulesForAllRequests=""true"">
      <add name=""EchoHttpModule"" type=""IisExpressTestKit.EchoHttpModule, IisExpressTestKit"" />
    </modules>
  </system.webServer>
</configuration>";

        public string ConfigTransformPath { get; set; }
        
        public void Start()
        {
            PrepareForStart();

            var iisExpress = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), IisExpressExe);

            _process = Process.Start(new ProcessStartInfo(iisExpress, $"/path:\"{_wwwroot}\" /port:{_port} /systray:false")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });

            WaitForStartup();
        }

        public void Stop()
        {
            _process?.Kill();
            _process?.Dispose();

            _process = null;
        }

        public void Dispose()
        {
            Stop();
        }

        public IisExpressResponse Request(string path, string originalFile = null)
        {
            var response = ExecuteRequest(FormatAbsoluteUrl(path), originalFile);

            var data = new IisExpressResponse
            {
                Host = response.Headers["X-IIS-RealHost"],
                Path = response.Headers["X-IIS-RealPath"],
                StatusCode = response.StatusCode,
                Headers = new NameValueCollection(response.Headers)
            };

            if (!string.IsNullOrEmpty(data.Headers["Location"]))
            {
                data.Headers["Location"] = data.Headers["Location"].Replace(FormatAbsoluteUrl(), "");
            }

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                data.Body = reader.ReadToEnd();
            }

            return data;
        }

        private void PrepareForStart()
        {
            if (!Directory.Exists(_wwwroot))
            {
                Directory.CreateDirectory(_wwwroot);
            }

            var binDirectory = Path.Combine(_wwwroot, "bin");

            if (!Directory.Exists(binDirectory))
            {
                Directory.CreateDirectory(binDirectory);
            }

            CopyFileToDirectory(ConfigTransformPath, _wwwroot);
            CopyFileToDirectory(typeof(IisExpress).Assembly.Location, binDirectory);

            var document = new XmlTransformableDocument();

            document.Load(new StringReader(WebConfigTemplate));

            var transform = new XmlTransformation(ConfigTransformPath);

            transform.Apply(document);

            document.Save(Path.Combine(_wwwroot, "Web.config"));
        }

        private void WaitForStartup()
        {
            var client = new WebClient();

            while (true)
            {
                try
                {
                    client.DownloadString(FormatAbsoluteUrl());

                    break;
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = (HttpWebResponse)ex.Response;

                        if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            Stop();

                            throw;
                        }

                        break;
                    }
                }

                Thread.Sleep(100);
            }
        }

        private string FormatAbsoluteUrl(string path = "")
        {
            return $"http://localhost:{_port}{path}";
        }

        private static HttpWebResponse ExecuteRequest(string url, string originalFile)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.AllowAutoRedirect = false;

            if (!string.IsNullOrEmpty(originalFile))
            {
                request.Headers["X-OriginalFile"] = Path.GetFullPath(originalFile);
            }

            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
        }

        private static void CopyFileToDirectory(string sourceFilePath, string destinationDirectory)
        {
            var fileName = Path.GetFileName(sourceFilePath);

            File.Copy(sourceFilePath, Path.Combine(destinationDirectory, fileName), true);
        }

        private static int GetPortNumber()
        {
            int port;

            do
            {
                port = _random.Next(1025, 65535);
            } while (!IsPortAvailable(port));

            return port;
        }

        private static bool IsPortAvailable(int port)
        {
            var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();

            return connections.All(x => x.LocalEndPoint.Port != port);
        }
    }
}
