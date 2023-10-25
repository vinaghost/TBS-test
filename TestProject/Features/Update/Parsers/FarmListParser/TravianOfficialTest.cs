using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Features.Update.Parsers.FarmListParser;

namespace TestProject.Features.Update.Parsers.FarmListParser
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

            dto.Count().Should().Be(8);
        }

        [TestMethod]
        public void Get_Content_Correct()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Id.Should().Be(new FarmListId(1233));
            dto.Name.Should().Be("Inactive");
        }
    }
}