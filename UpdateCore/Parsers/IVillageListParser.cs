﻿using HtmlAgilityPack;

namespace UpdateCore.Parsers
{
    public interface IVillageListParser
    {
        int GetId(HtmlNode node);

        string GetName(HtmlNode node);

        List<HtmlNode> GetVillages(HtmlDocument doc);

        int GetX(HtmlNode node);

        int GetY(HtmlNode node);

        bool IsUnderAttack(HtmlNode node);
    }
}