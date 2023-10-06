using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.HeroParser;

namespace TestProject.Features.Update.Parsers.HeroParser
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

        [DataTestMethod]
        public void Get_Count_Correct()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inventory.html");
            html.Load(path);
            var dto = parser.GetItems(html);

            dto.Count().Should().Be(9);
        }
    }
}