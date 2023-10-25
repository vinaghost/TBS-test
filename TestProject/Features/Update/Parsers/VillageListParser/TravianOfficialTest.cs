using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Features.Update.Parsers.VillageListParser;

namespace TestProject.Features.Update.Parsers.VillageListParser
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
        public void Get_Count_Correct()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(15);
        }

        [TestMethod]
        public void Get_Content_Correct()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Id.Should().Be(new VillageId(19501));
            dto.Name.Should().Be("VinaVillage");
            dto.X.Should().Be(114);
            dto.Y.Should().Be(-32);
            dto.IsActive.Should().Be(false);
            dto.IsUnderAttack.Should().Be(false);
        }
    }
}