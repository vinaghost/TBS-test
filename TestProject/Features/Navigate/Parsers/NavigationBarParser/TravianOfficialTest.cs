using HtmlAgilityPack;
using MainCore.Features.Navigate.Parsers.NavigationBarParser;

namespace TestProject.Features.Navigate.Parsers.NavigationBarParser
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
        public void GetBuildingButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetBuildingButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetDailyButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetDailyButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetMapButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetMapButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetMessageButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetMessageButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetReportsButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetReportsButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetResourceButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetResourceButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetStatisticsButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetStatisticsButton(html);
            Assert.IsNotNull(node);
        }
    }
}