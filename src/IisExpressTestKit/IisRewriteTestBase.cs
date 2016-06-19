using Xunit;

namespace IisExpressTestKit
{
    public abstract class IisRewriteTestBase : IClassFixture<IisExpressFixture>
    {
        protected IisRewriteTestBase(IisExpressFixture fixture)
        {
            Iis = fixture.Iis;
        }

        protected IisExpress Iis { get; }
    }
}
