﻿using HtmlAgilityPack;

namespace UpdateCore.Parsers
{
    public class VillageListParser : IVillageListParser
    {
        public List<HtmlNode> GetVillages(HtmlDocument doc)
        {
            var villsNode = doc.GetElementbyId("sidebarBoxVillagelist");
            if (villsNode is null) return new();
            return villsNode.Descendants("div").Where(x => x.HasClass("listEntry")).ToList();
        }

        public bool IsUnderAttack(HtmlNode node)
        {
            return node.HasClass("attack");
        }

        public int GetId(HtmlNode node)
        {
            var dataDid = node.GetAttributeValue("data-did", 0);
            return dataDid;
        }

        public string GetName(HtmlNode node)
        {
            var textNode = node.Descendants("a").FirstOrDefault();
            if (textNode is null) return "";
            var nameNode = textNode.Descendants("span").FirstOrDefault(x => x.HasClass("name"));
            if (nameNode is null) return "";
            return nameNode.InnerText;
        }

        public int GetX(HtmlNode node)
        {
            var xNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateX"));
            if (xNode is null) return 0;
            var xStr = new string(xNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('−')).ToArray());
            xStr = xStr.Replace('−', '-');
            if (string.IsNullOrEmpty(xStr)) return 0;

            return int.Parse(xStr);
        }

        public int GetY(HtmlNode node)
        {
            var yNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateY"));
            if (yNode is null) return 0;
            var yStr = new string(yNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('−')).ToArray());
            yStr = yStr.Replace('−', '-');
            if (string.IsNullOrEmpty(yStr)) return 0;
            return int.Parse(yStr);
        }

        public bool IsActive(HtmlNode node)
        {
            return node.HasClass("active");
        }
    }
}