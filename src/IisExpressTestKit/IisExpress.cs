using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using Newtonsoft.Json;

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
    <rewrite>
      <rules configSource=""{0}"" />
    </rewrite>
  </system.webServer>
</configuration>";

        public string RewriteConfigPath { get; set; }
        
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

        public IisExpressResponse Request(string path)
        {
            var response = ExecuteRequest(AbsoluteUrl(path));
            
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                IisExpressResponse data;

                try
                {
                    data = JsonConvert.DeserializeObject<IisExpressResponse>(reader.ReadToEnd());
                }
                catch
                {
                    data = new IisExpressResponse();
                }

                data.StatusCode = response.StatusCode;
                data.Headers = new NameValueCollection(response.Headers);

                if (!string.IsNullOrEmpty(data.Headers["Location"]))
                {
                    data.Headers["Location"] = data.Headers["Location"].Replace(AbsoluteUrl(), "");
                }

                return data;
            }
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

            CopyFileToDirectory(RewriteConfigPath, _wwwroot);
            CopyFileToDirectory(typeof(IisExpress).Assembly.Location, binDirectory);
            CopyFileToDirectory(typeof(JsonConvert).Assembly.Location, binDirectory);

            var contents = string.Format(WebConfigTemplate, Path.GetFileName(RewriteConfigPath));

            File.WriteAllText(Path.Combine(_wwwroot, "Web.config"), contents);
        }

        private void WaitForStartup()
        {
            var client = new WebClient();

            while (true)
            {
                try
                {
                    client.DownloadString(AbsoluteUrl());

                    break;
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = (HttpWebResponse)ex.Response;

                        if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            throw;
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }

        private string AbsoluteUrl(string path = "")
        {
            return $"http://localhost:{_port}{path}";
        }

        private static HttpWebResponse ExecuteRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.AllowAutoRedirect = false;

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
