using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.AccountInfoParser;

namespace TestProject.Features.Update.Parsers.AccountInfoParser
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
        [DataRow(37)]
        public void GetGold_Vailidate_Number(int expected)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var result = parser.GetGold(html);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(562)]
        public void GetSilver_Vailidate_Number(int expected)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var result = parser.GetSilver(html);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(false)]
        public void HasPlusAccount_Vailidate_Boolean(bool expected)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var result = parser.HasPlusAccount(html);
            Assert.AreEqual(expected, result);
        }
    }
}