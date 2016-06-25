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

            Iis.Request("/hoge/foo/bar/baz")
               .IsPath("/translated/hoge/foo/bar/baz")
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

        [Fact]
        public void StaticFileのテスト()
        {
            Iis.Request("/test", "outbound.html")
               .IsHeaderValue("Content-Type", "text/html")
               .IsStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public void OutboundRuleのテスト()
        {
            Iis.Request("/outbound.html", @".\outbound.html")
               .IsHeaderValue("Content-Type", "text/html")
               .HtmlAttribute("a", "href", "/translated/hoge")
               .IsStatusCode(HttpStatusCode.OK);
        }
    }
}
