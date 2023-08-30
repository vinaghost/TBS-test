using HtmlAgilityPack;

namespace ParserCore.Parser
{
    public interface ILoginPageParser
    {
        HtmlNode GetLoginButton(HtmlDocument doc);
        HtmlNode GetPasswordNode(HtmlDocument doc);
        HtmlNode GetUsernameNode(HtmlDocument doc);
    }
}