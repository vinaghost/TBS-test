﻿using FluentAssertions;
using MainCore.Common.Enums;
using MainCore.Parsers.HeroParser;

namespace TestProject.Parsers.HeroParser
{
    [TestClass]
    public class TravianOfficialTest : ParserTestBase<TravianOfficial>
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            parts = Helper.GetParts<TravianOfficialTest>();
        }

        [TestMethod]
        public void Get_Count_ShouldBeCorrect()
        {
            var (parser, html) = Setup("TravianOfficial_inventory.html");
            var dto = parser.Get(html);

            dto.Count().Should().Be(13);
        }

        [TestMethod]
        public void Get_Content_ShouldBeCorrect()
        {
            var (parser, html) = Setup("TravianOfficial_inventory.html");

            var dto = parser.Get(html).FirstOrDefault();

            dto.Type.Should().Be(HeroItemEnums.Wood);
            dto.Amount.Should().Be(118_226);
        }

        [TestMethod]
        public void GetAmountBox_ShoulNotBeNull()
        {
            var (parser, html) = Setup("TravianOfficial_dialog.html");
            var node = parser.GetAmountBox(html);

            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetConfirmButton_ShoulNotBeNull()
        {
            var (parser, html) = Setup("TravianOfficial_dialog.html");
            var node = parser.GetConfirmButton(html);

            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetHeroAvatar_ShoulNotBeNull()
        {
            var (parser, html) = Setup("TravianOfficial_inventory.html");
            var node = parser.GetHeroAvatar(html);

            node.Should().NotBeNull();
        }

        [TestMethod]
        public void InventoryTabActive_ShoulBeTrue()
        {
            var (parser, html) = Setup("TravianOfficial_inventory.html");
            var node = parser.InventoryTabActive(html);

            node.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(HeroItemEnums.Wood)]
        [DataRow(HeroItemEnums.Clay)]
        [DataRow(HeroItemEnums.Iron)]
        [DataRow(HeroItemEnums.Crop)]
        public void GetItemSlot_ShoulNotBeNull(HeroItemEnums type)
        {
            var (parser, html) = Setup("TravianOfficial_inventory.html");
            var node = parser.GetItemSlot(html, type);

            node.Should().NotBeNull();
        }
    }
}