using FluentAssertions;
using HtmlAgilityPack;
using MainCore.Features.Update.Parsers.AccountInfoParser;

namespace TestProject.Features.Update.Parsers.AccountInfoParser
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
        [DataRow(37)]
        public void GetGold_Vailidate_Number(int expected)
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