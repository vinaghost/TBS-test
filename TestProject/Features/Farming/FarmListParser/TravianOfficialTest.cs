using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Features.Farming.Parsers.FarmListParser;

namespace TestProject.Features.Farming.FarmListParser
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
        public void GetStartButton_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetStartButton(html, new FarmId(1233));
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetStartAllButton_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetStartAllButton(html);
            node.Should().NotBeNull();
        }
    }
}