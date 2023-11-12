using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Parsers.LoginPageParser;

namespace TestProject.Parsers.LoginPageParser
{
    [TestClass]
    public class TTWarsTest : ParserTestBase<TTWars>
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            parts = Helper.GetParts<TravianOfficialTest>();
        }

        [TestMethod]
        public void GetUsernameNode_Vailidate_ShouldBeNotNull()
        {
            var (parser, html) = Setup("TTWars.html");

            var node = parser.GetUsernameNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetPasswordNode_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetPasswordNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetLoginButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetLoginButton(html);
            node.Should().NotBeNull();
        }
    }
}