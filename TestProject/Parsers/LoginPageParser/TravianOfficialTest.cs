using FluentAssertions;
using MainCore.Parsers.LoginPageParser;

namespace TestProject.Parsers.LoginPageParser
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
        public void GetUsernameNode_ShouldBeNotNull()
        {
            var (parser, html) = Setup("TravianOfficial.html");

            var node = parser.GetUsernameNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetPasswordNode_ShouldBeNotNull()
        {
            var (parser, html) = Setup("TravianOfficial.html");

            var node = parser.GetPasswordNode(html);
            node.Should().NotBeNull();
        }

        [TestMethod]
        public void GetLoginButton_ShouldBeNotNull()
        {
            var (parser, html) = Setup("TravianOfficial.html");

            var node = parser.GetLoginButton(html);
            node.Should().NotBeNull();
        }
    }
}