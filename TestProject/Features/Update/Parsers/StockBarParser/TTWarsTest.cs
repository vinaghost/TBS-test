using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.StockBarParser;

namespace TestProject.Features.Update.Parsers.StockBarParser
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

        [DataTestMethod]
        public void Get_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Wood.Should().Be(6_000_000);
            dto.Clay.Should().Be(6_000_000);
            dto.Iron.Should().Be(6_000_000);
            dto.Crop.Should().Be(6_000_000);
            dto.FreeCrop.Should().Be(20_319_811);
            dto.Warehouse.Should().Be(6_000_000);
            dto.Granary.Should().Be(6_000_000);
        }
    }
}