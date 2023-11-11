using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Parsers.InfrastructureParser;

namespace TestProject.Parsers.InfrastructureParser
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
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(22);
        }

        [TestMethod]
        public void Get_Content_ShouldBeCorrect()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Type.Should().Be(BuildingEnums.Site);
            dto.Location.Should().Be(19);
            dto.Level.Should().Be(-1);
            dto.IsUnderConstruction.Should().Be(false);
        }
    }
}