﻿using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Features.Update.Parsers.FieldParser;

namespace TestProject.Features.Update.Parsers.FieldParser
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
        public void Get_Count_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(18);
        }

        [TestMethod]
        public void Get_Content_Correct()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Type.Should().Be(BuildingEnums.Woodcutter);
            dto.Location.Should().Be(1);
            dto.Level.Should().Be(10);
            dto.IsUnderConstruction.Should().Be(false);
        }
    }
}