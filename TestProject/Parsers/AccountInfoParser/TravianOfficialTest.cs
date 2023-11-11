using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Parsers.AccountInfoParser;

namespace TestProject.Parsers.AccountInfoParser
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
        public void Get_ShouldBeCorrect()
        {
            var parser = new TravianOfficial();
            var html = new HtmlDocument();
            var path = Helper.GetPath(parts, "TravianOfficial.html");
            html.Load(path);
            var dto = parser.Get(html);

            dto.Gold.Should().Be(37);
            dto.Silver.Should().Be(562);
            dto.HasPlusAccount.Should().Be(true);
        }
    }
}