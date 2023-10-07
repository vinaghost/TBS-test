using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Features.Navigate.Parsers.HeroParser;

namespace TestProject.Features.Navigate.Parsers.HeroParser
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
        public void GetAmountBox_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inputdialog.html");
            html.Load(path);

            var node = parser.GetAmountBox(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetConfirmButton_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inputdialog.html");
            html.Load(path);

            var node = parser.GetConfirmButton(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetHeroAvatar_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inputdialog.html");
            html.Load(path);

            var node = parser.GetHeroAvatar(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetHeroTab_NotNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inventory.html");
            html.Load(path);

            var node = parser.GetHeroTab(html, 0);
            node.Should().NotBeNull();
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(2, false)]
        public void IsCurrentTab_Boolean(int index, bool expected)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inventory.html");
            html.Load(path);

            var node = parser.GetHeroTab(html, index);
            var result = parser.IsCurrentTab(node);
            result.Should().Be(expected);
        }

        [TestMethod]
        [DataRow(HeroItemEnums.Wood)]
        [DataRow(HeroItemEnums.Iron)]
        public void GetItemSlot_Type_NotNull(HeroItemEnums type)
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_inventory.html");
            html.Load(path);

            var node = parser.GetItemSlot(html, type);
            node.Should().NotBeNull();
        }
    }
}