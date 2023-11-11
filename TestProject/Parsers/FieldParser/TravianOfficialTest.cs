using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Parsers.FieldParser;

namespace TestProject.Parsers.FieldParser
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
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(18);
        }

        [TestMethod]
        public void Get_Content_ShouldBeCorrect()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Type.Should().Be(BuildingEnums.Woodcutter);
            dto.Location.Should().Be(1);
            dto.Level.Should().Be(10);
            dto.IsUnderConstruction.Should().Be(false);
        }
    }
}