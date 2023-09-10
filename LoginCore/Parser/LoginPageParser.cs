﻿using HtmlAgilityPack;

namespace LoginCore.Parsers
{
    public class LoginPageParser : ILoginPageParser
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
            var trNode = doc.DocumentNode.Descendants("tr").FirstOrDefault(x => x.HasClass("loginButtonRow"));
            if (trNode is null) return null;
            return trNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
        }
    }
}