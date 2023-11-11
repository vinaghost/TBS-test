using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Entities;
using MainCore.Parsers.FarmParser;

namespace TestProject.Parsers.FarmParser
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
        public void GetStartButton_ShouldNotBeNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetStartButton(html, new FarmId(54));
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetStartAllButton_ShouldNotBeNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetStartAllButton(html);
            node.Should().BeNull();
        }

        [TestMethod]
        public void Get_Count_ShouldBeCorrect()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Count().Should().Be(5);
        }

        [TestMethod]
        public void Get_Content_ShouldBeCorrect()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);
            var dto = parser.Get(html).FirstOrDefault();

            dto.Id.Should().Be(new FarmId(54));
            dto.Name.Should().Be("1");
        }
    }
}