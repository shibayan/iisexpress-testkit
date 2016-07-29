using System;
using System.Configuration;
using System.IO;

namespace IisExpressTestKit
{
    public class IisExpressFixture : IDisposable
    {
        public IisExpressFixture()
        {
            Iis = new IisExpress
            {
                ConfigTransformPath = ConfigurationManager.AppSettings["ConfigTransformPath"] ?? Path.Combine("Transform.config")
            };

            Iis.Start();
        }

        public void Dispose()
        {
            Iis.Stop();
        }

        public IisExpress Iis { get; }
    }
}
