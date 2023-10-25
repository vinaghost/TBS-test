using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Features.Farming.Parsers.FarmListParser;

namespace TestProject.Features.Farming.FarmListParser
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
        public void GetStartButton_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetStartButton(html, new FarmListId(54));
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetStartAllButton_Null()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetStartAllButton(html);
            node.Should().BeNull();
        }
    }
}