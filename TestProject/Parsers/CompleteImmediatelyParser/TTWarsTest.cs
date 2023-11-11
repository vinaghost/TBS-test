using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Parsers.CompleteImmediatelyParser;

namespace TestProject.Parsers.CompleteImmediatelyParser
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
        public void GetCompleteButton_ShouldNotBeNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var node = parser.GetCompleteButton(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetConfirmButton_ShouldNotBeNull()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars_dialogue.html");
            html.Load(path);

            var node = parser.GetConfirmButton(html);
            node.Should().NotBeNull();
        }
    }
}