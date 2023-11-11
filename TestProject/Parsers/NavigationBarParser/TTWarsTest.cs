using HtmlAgilityPack;
using MainCore.Parsers.NavigationBarParser;

namespace TestProject.Parsers.NavigationBarParser
{
    [TestClass]
    public class TTWarsTest
    {
        private static string[] parts;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            parts = Helper.GetParts<TTWarsTest>();
        }

        [TestMethod]
        public void GetBuildingButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetBuildingButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetDailyButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetDailyButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetMapButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetMapButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetMessageButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetMessageButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetReportsButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetReportsButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetResourceButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetResourceButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetStatisticsButton_Vailidate_ShouldBeNotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetStatisticsButton(html);
            Assert.IsNotNull(node);
        }
    }
}