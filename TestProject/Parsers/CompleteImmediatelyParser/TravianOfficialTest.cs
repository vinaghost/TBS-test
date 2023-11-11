using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Parsers.CompleteImmediatelyParser;

namespace TestProject.Parsers.CompleteImmediatelyParser
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
        public void GetCompleteButton_ShouldNotBeNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);

            var node = parser.GetCompleteButton(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetConfirmButton_ShouldNotBeNull()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial_dialogue.html");
            html.Load(path);

            var node = parser.GetConfirmButton(html);
            node.Should().NotBeNull();
        }
    }
}