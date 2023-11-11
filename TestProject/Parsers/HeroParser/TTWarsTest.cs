using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Parsers.HeroParser;

namespace TestProject.Parsers.HeroParser
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
        public void Get_Count_ShouldBeCorrect()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inventory.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(6);
        }

        [TestMethod]
        public void Get_Content_ShouldBeCorrect()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inventory.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Type.Should().Be(HeroItemEnums.Wood);
            dto.Amount.Should().Be(61_576_323);
        }
    }
}