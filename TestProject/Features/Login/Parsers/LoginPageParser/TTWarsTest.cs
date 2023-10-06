using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Login.Parsers.LoginPageParser;

namespace TestProject.Features.Login.Parsers.LoginPageParser
{
    [TestClass]
    public class TTWarsTest
    {
        private static string[] parts;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            parts = Helper.GetParts<TravianOfficialTest>();
        }

        [TestMethod]
        public void GetUsernameNode_Vailidate_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetUsernameNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetPasswordNode_Vailidate_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetPasswordNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetLoginButton_Vailidate_NotNull()
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