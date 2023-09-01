using HtmlAgilityPack;

namespace LoginCore.Parser
{
    public interface ILoginPageParser
    {
        HtmlNode GetLoginButton(HtmlDocument doc);

        HtmlNode GetPasswordNode(HtmlDocument doc);

        HtmlNode GetUsernameNode(HtmlDocument doc);
    }
}