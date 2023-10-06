﻿using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Farming.Parsers.FarmListParser
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

        public HtmlNode GetStartAllButton(HtmlDocument doc)
        {
            return null;
        }
    }
}