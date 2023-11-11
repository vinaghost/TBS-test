using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Parsers.LoginPageParser;

namespace TestProject.Parsers.LoginPageParser
{
    [TestClass]
    public class TravianOfficialTest
    {
        private static string[] parts;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            parts = Helper.GetParts<TravianOfficialTest>();
        }

        [TestMethod]
        public void GetUsernameNode_ShouldBeNotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetUsernameNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetPasswordNode_ShouldBeNotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetPasswordNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetLoginButton_ShouldBeNotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetLoginButton(html);
            node.Should().NotBeNull();
        }
    }
}