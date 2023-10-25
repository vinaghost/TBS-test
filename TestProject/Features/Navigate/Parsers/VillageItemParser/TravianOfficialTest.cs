using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Features.Navigate.Parsers.VillageItemParser;

namespace TestProject.Features.Navigate.Parsers.VillageItemParser
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
        [DataRow(19501)]
        [DataRow(21180)]
        public void GetVillageNode_NotNull(int villageId)
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetVillageNode(html, new VillageId(villageId));
            node.Should().NotBeNull();
        }

        [TestMethod]
        [DataRow(19501, true)]
        [DataRow(21180, false)]
        public void IsActive_Vailidate_NotNull(int villageId, bool expected)
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetVillageNode(html, new VillageId(villageId));
            var result = parser.IsActive(node);
            Assert.AreEqual(expected, result);
        }
    }
}