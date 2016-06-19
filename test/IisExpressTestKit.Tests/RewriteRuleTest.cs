using System.Net;

using Xunit;

namespace IisExpressTestKit.Tests
{
    public class RewriteRuleTest : IisRewriteTestBase
    {
        public RewriteRuleTest(IisExpressFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public void Rewriteルールのテスト()
        {
            Iis.Request("/hoge")
               .IsPath("/translated/hoge")
               .IsStatusCode(HttpStatusCode.OK);

            Iis.Request("/foo/bar/baz")
               .IsPath("/translated/foo/bar/baz")
               .IsStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public void Redirectのテスト()
        {
            Iis.Request("/found")
               .IsRedirect("http://www.google.co.jp");
        }

        [Fact]
        public void PermanentRedirectのテスト()
        {
            Iis.Request("/redirect")
               .IsRedirect("http://www.google.co.jp")
               .IsStatusCode(HttpStatusCode.MovedPermanently);
        }

        [Fact]
        public void LocalRedirectのテスト()
        {
            Iis.Request("/local")
               .IsRedirect("/local/redirect");
        }

        [Fact]
        public void StatusCodeのテスト()
        {
            Iis.Request("/404")
               .IsStatusCode(HttpStatusCode.NotFound);
        }
    }
}
