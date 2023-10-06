using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.QueueBuildingParser;

namespace TestProject.Features.Update.Parsers.QueueBuildingParser
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
        public void Get_Correct()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html).ToList();

            dto.Count.Should().Be(4);
            dto[0].Level.Should().Be(13);
            dto[3].Level.Should().Be(-1);
        }
    }
}