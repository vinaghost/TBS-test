﻿using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Features.Update.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Update.Parsers.VillageListParser
{
    [RegisterAsTransient(ServerEnums.TravianOfficial)]
    public class TravianOfficial : IVillageListParser
    {
        public IEnumerable<VillageDto> Get(HtmlDocument doc)
        {
            var nodes = GetVillages(doc);
            foreach (var node in nodes)
            {
                var id = GetId(node);
                var name = GetName(node);
                var x = GetX(node);
                var y = GetY(node);
                var isActive = IsActive(node);
                var isUnderAttack = IsUnderAttack(node);
                yield return new()
                {
                    Id = id,
                    Name = name,
                    X = x,
                    Y = y,
                    IsActive = isActive,
                    IsUnderAttack = isUnderAttack,
                };
            }
        }

        private static List<HtmlNode> GetVillages(HtmlDocument doc)
        {
            var villsNode = doc.GetElementbyId("sidebarBoxVillagelist");
            if (villsNode is null) return new();
            return villsNode.Descendants("div").Where(x => x.HasClass("listEntry")).ToList();
        }

        private static bool IsUnderAttack(HtmlNode node)
        {
            return node.HasClass("attack");
        }

        private static int GetId(HtmlNode node)
        {
            var dataDid = node.GetAttributeValue("data-did", 0);
            return dataDid;
        }

        private static string GetName(HtmlNode node)
        {
            var textNode = node.Descendants("a").FirstOrDefault();
            if (textNode is null) return "";
            var nameNode = textNode.Descendants("span").FirstOrDefault(x => x.HasClass("name"));
            if (nameNode is null) return "";
            return nameNode.InnerText;
        }

        private static int GetX(HtmlNode node)
        {
            var xNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateX"));
            if (xNode is null) return 0;
            var xStr = new string(xNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('−')).ToArray());
            xStr = xStr.Replace('−', '-');
            if (string.IsNullOrEmpty(xStr)) return 0;

            return int.Parse(xStr);
        }

        private static int GetY(HtmlNode node)
        {
            var yNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateY"));
            if (yNode is null) return 0;
            var yStr = new string(yNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('−')).ToArray());
            yStr = yStr.Replace('−', '-');
            if (string.IsNullOrEmpty(yStr)) return 0;
            return int.Parse(yStr);
        }

        private static bool IsActive(HtmlNode node)
        {
            return node.HasClass("active");
        }
    }
}