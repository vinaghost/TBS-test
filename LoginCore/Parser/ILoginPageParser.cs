using HtmlAgilityPack;

namespace LoginCore.Parsers
{
    public interface ILoginPageParser
    {
        HtmlNode GetLoginButton(HtmlDocument doc);

        HtmlNode GetPasswordNode(HtmlDocument doc);

        HtmlNode GetUsernameNode(HtmlDocument doc);
    }
}