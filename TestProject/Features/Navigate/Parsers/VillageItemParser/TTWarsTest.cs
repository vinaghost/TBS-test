using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Features.Navigate.Parsers.VillageItemParser;

namespace TestProject.Features.Navigate.Parsers.VillageItemParser
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
        [DataRow(255_147)]
        public void GetVillageNode_NotNull(int villageId)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetVillageNode(html, new VillageId(villageId));
            node.Should().NotBeNull();
        }

        [TestMethod]
        [DataRow(255147, true)]
        public void IsActive_NotNull(int villageId, bool expected)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetVillageNode(html, new VillageId(villageId));
            var result = parser.IsActive(node);
            result.Should().Be(expected);
        }
    }
}