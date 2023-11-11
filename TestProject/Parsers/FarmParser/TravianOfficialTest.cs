﻿using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Parsers.FarmParser;

namespace TestProject.Parsers.FarmParser
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
        public void GetStartButton_ShouldNotBeNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetStartButton(html, new FarmId(1233));
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetStartAllButton_ShouldNotBeNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetStartAllButton(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void Get_Count_ShouldBeCorrect()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(8);
        }

        [TestMethod]
        public void Get_Content_ShouldBeCorrect()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Id.Should().Be(new FarmId(1233));
            dto.Name.Should().Be("Inactive");
        }
    }
}