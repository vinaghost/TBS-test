﻿using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Parsers;

namespace MainCore.Parsers.NavigationBarParser
{
    [RegisterAsTransient(ServerEnums.TravianOfficial)]
    public class TravianOfficial : INavigationBarParser
    {
        private static HtmlNode GetButton(HtmlDocument doc, int key)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == key);
            return buttonNode;
        }

        public HtmlNode GetResourceButton(HtmlDocument doc) => GetButton(doc, 1);

        public HtmlNode GetBuildingButton(HtmlDocument doc) => GetButton(doc, 2);

        public HtmlNode GetMapButton(HtmlDocument doc) => GetButton(doc, 3);

        public HtmlNode GetStatisticsButton(HtmlDocument doc) => GetButton(doc, 4);

        public HtmlNode GetReportsButton(HtmlDocument doc) => GetButton(doc, 5);

        public HtmlNode GetMessageButton(HtmlDocument doc) => GetButton(doc, 6);

        public HtmlNode GetDailyButton(HtmlDocument doc) => GetButton(doc, 7);
    }
}