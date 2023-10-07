using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Features.Update.Parsers.HeroParser;

namespace TestProject.Features.Update.Parsers.HeroParser
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
        public void Get_Count_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inventory.html");
            html.Load(path);
            var dto = parser.GetItems(html);

            dto.Count().Should().Be(6);
        }

        [TestMethod]
        public void Get_Content_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inventory.html");
            html.Load(path);
            var dto = parser.GetItems(html).FirstOrDefault();

            dto.Type.Should().Be(HeroItemEnums.Wood);
            dto.Amount.Should().Be(61_576_323);
        }
    }
}