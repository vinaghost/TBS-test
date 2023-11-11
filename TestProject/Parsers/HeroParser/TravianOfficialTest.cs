using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Parsers.HeroParser;

namespace TestProject.Parsers.HeroParser
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
        public void Get_Count_ShouldBeCorrect()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inventory.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(9);
        }

        [TestMethod]
        public void Get_Content_ShouldBeCorrect()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inventory.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Type.Should().Be(HeroItemEnums.Wood);
            dto.Amount.Should().Be(86_197);
        }
    }
}