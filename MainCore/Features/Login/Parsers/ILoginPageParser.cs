using HtmlAgilityPack;

namespace MainCore.Features.Login.Parsers
{
    public interface ILoginPageParser
    {
        HtmlNode GetLoginButton(HtmlDocument doc);

        HtmlNode GetPasswordNode(HtmlDocument doc);

        HtmlNode GetUsernameNode(HtmlDocument doc);
    }
}