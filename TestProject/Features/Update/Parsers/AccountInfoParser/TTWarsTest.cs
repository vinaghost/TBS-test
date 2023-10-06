using FluentAssertions;
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
        public void Get_ShouldReturnCorrect()
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