using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.StockBarParser;

namespace TestProject.Features.Update.Parsers.StockBarParser
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
        public void Get_Correct()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Wood.Should().Be(173_604);
            dto.Clay.Should().Be(129);
            dto.Iron.Should().Be(255_036);
            dto.Crop.Should().Be(640_000);
            dto.FreeCrop.Should().Be(74_061);
            dto.Warehouse.Should().Be(320_000);
            dto.Granary.Should().Be(640_000);
        }
    }
}