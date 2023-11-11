using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Parsers.AccountInfoParser;

namespace TestProject.Parsers.AccountInfoParser
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
        public void Get_ShouldBeCorrect()
        {
            var parser = new TTWars();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TTWars.html");
            html.Load(path);

            var dto = parser.Get(html);

            dto.Gold.Should().Be(210);
            dto.Silver.Should().Be(0);
            dto.HasPlusAccount.Should().Be(false);
        }
    }
}