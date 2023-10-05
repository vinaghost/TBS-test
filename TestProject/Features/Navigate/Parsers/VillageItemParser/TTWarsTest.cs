using HtmlAgilityPack;
using MainCore.Features.Navigate.Parsers.VillageItemParser;

namespace TestProject.Features.Navigate.Parsers.VillageItemParser
{
    [TestClass]
    public class TTWarsTest
    {
        private static string[] parts;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            parts = Helper.GetParts<TTWarsTest>();
        }

        [DataTestMethod]
        [DataRow(255147)]
        public void GetVillageNode_Vailidate_NotNull(int villageId)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetVillageNode(html, villageId);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(255147, true)]
        public void IsActive_Vailidate_NotNull(int villageId, bool expected)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetVillageNode(html, villageId);
            var result = parser.IsActive(node);
            Assert.AreEqual(expected, result);
        }
    }
}