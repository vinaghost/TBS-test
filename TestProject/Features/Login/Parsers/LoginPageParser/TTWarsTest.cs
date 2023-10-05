using HtmlAgilityPack;
using MainCore.Features.Login.Parsers.LoginPageParser;

namespace TestProject.Features.Login.Parsers.LoginPageParser
{
    [TestClass]
    public class TTWarsTest
    {
        private static string[] parts;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            parts = Helper.GetParts<TravianOfficialTest>();
        }

        [TestMethod]
        public void GetUsernameNode_Vailidate_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetUsernameNode(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetPasswordNode_Vailidate_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetPasswordNode(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetLoginButton_Vailidate_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetLoginButton(html);
            Assert.IsNotNull(node);
        }
    }
}