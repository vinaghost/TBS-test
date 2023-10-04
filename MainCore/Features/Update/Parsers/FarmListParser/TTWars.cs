﻿using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Update.Parsers.FarmListParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IFarmListParser
    {
        public HtmlNode GetStartButton(HtmlDocument doc, int raidId)
        {
            var farmNode = doc.GetElementbyId($"raidList{raidId}");
            if (farmNode is null) return null;
            var startNode = farmNode.Descendants("button")
                                    .FirstOrDefault(x => x.HasClass("startButton"));
            return startNode;
        }

        public List<HtmlNode> GetFarmNodes(HtmlDocument doc)
        {
            var raidList = doc.GetElementbyId("raidList");
            if (raidList is null) return new();
            var fls = raidList.Descendants("div").Where(x => x.HasClass("raidList"));

            return fls.ToList();
        }

        public string GetName(HtmlNode node)
        {
            var flName = node.Descendants("div").FirstOrDefault(x => x.HasClass("listName"));
            if (flName is null) return null;
            var name = flName.Descendants("span").FirstOrDefault(x => x.HasClass("value"));
            if (name is null) return null;
            return name.InnerText.Trim();
        }

        public int GetId(HtmlNode node)
        {
            var id = node.Id;
            var value = new string(id.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        public HtmlNode GetStartAllButton(HtmlDocument doc)
        {
            return null;
        }
    }
}