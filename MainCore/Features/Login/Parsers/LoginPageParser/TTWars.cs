using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Login.Parsers.LoginPageParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : ILoginPageParser
    {
        public HtmlNode GetUsernameNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("name"));
        }

        public HtmlNode GetPasswordNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("password"));
        }

        public HtmlNode GetLoginButton(HtmlDocument doc)
        {
            return doc.GetElementbyId("s1");
        }
    }
}