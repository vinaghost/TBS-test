using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.QueueBuildingParser;

namespace TestProject.Features.Update.Parsers.QueueBuildingParser
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
        public void Get_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html).ToList();

            dto.Count.Should().Be(4);
        }

        [TestMethod]
        public void Get_Content_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Position.Should().Be(0);
            dto.Location.Should().Be(-1);
            dto.Level.Should().Be(8);
            dto.Type.Should().Be("MainBuilding");
        }
    }
}