using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Features.Update.Parsers.InfrastructureParser;

namespace TestProject.Features.Update.Parsers.InfrastructureParser
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

            dto.Count().Should().Be(22);
        }

        [TestMethod]
        public void Get_Content_Correct()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Type.Should().Be(BuildingEnums.Granary);
            dto.Location.Should().Be(19);
            dto.Level.Should().Be(20);
            dto.IsUnderConstruction.Should().Be(false);
        }
    }
}