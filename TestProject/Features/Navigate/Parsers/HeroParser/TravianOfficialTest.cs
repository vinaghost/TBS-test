using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Features.Navigate.Parsers.HeroParser;

namespace TestProject.Features.Navigate.Parsers.HeroParser
{
    [TestClass]
    public class TravianOfficialTest
    {
        private static string[] parts;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            parts = Helper.GetParts<TravianOfficialTest>();
        }

        [TestMethod]
        public void GetAmountBox_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inputdialog.html");
            html.Load(path);

            var node = parser.GetAmountBox(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetConfirmButton_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inputdialog.html");
            html.Load(path);

            var node = parser.GetConfirmButton(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetHeroAvatar_Vailidate_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inputdialog.html");
            html.Load(path);

            var node = parser.GetHeroAvatar(html);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetHeroTab_Index0_NotNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inventory.html");
            html.Load(path);

            var node = parser.GetHeroTab(html, 0);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(1, true)]
        [DataRow(2, false)]
        public void IsCurrentTab_Index_Boolean(int index, bool expected)
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inventory.html");
            html.Load(path);

            var node = parser.GetHeroTab(html, index);
            var result = parser.IsCurrentTab(node);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(HeroItemEnums.Wood)]
        [DataRow(HeroItemEnums.Iron)]
        public void GetItemSlot_Type_NotNull(HeroItemEnums type)
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_inventory.html");
            html.Load(path);

            var node = parser.GetItemSlot(html, type);
            Assert.IsNotNull(node);
        }
    }
}