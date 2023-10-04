﻿using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Update.Parsers.FieldParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IFieldParser
    {
        public List<HtmlNode> GetNodes(HtmlDocument doc)
        {
            var villageMapNode = doc.GetElementbyId("resourceFieldContainer");
            if (villageMapNode is null) return new();

            return villageMapNode.Descendants("div").Where(x => x.HasClass("level")).ToList();
        }

        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            return null;
        }

        public int GetId(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("buildingSlot"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;

            return int.Parse(strResult);
        }

        public BuildingEnums GetBuildingType(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("gid"));
            if (string.IsNullOrEmpty(needClass)) return BuildingEnums.Site;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return BuildingEnums.Site;

            return (BuildingEnums)int.Parse(strResult);
        }

        public int GetLevel(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("level") && !x.Equals("level"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());

            return int.Parse(strResult);
        }

        public bool IsUnderConstruction(HtmlNode node)
        {
            return node.GetClasses().Contains("underConstruction");
        }
    }
}