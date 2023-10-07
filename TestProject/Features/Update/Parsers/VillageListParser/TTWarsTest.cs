using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.VillageListParser;

namespace TestProject.Features.Update.Parsers.VillageListParser
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
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(1);
        }

        [TestMethod]
        public void Get_Content_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Id.Should().Be(255147);
            dto.Name.Should().Be("vinaghost`s village");
            dto.X.Should().Be(28);
            dto.Y.Should().Be(82);
            dto.IsActive.Should().Be(true);
            dto.IsUnderAttack.Should().Be(false);
        }
    }
}