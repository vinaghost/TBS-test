﻿using HtmlAgilityPack;

namespace UpdateCore.Parsers
{
    public interface IFarmListParser
    {
        List<HtmlNode> GetFarmNodes(HtmlDocument doc);
        int GetId(HtmlNode node);
        string GetName(HtmlNode node);
        HtmlNode GetStartAllButton(HtmlDocument doc);
        HtmlNode GetStartButton(HtmlDocument doc, int raidId);
    }
}