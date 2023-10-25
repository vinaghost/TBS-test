using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Farming.Parsers.FarmListParser
{
    [RegisterAsTransient(ServerEnums.TravianOfficial)]
    public class TravianOfficial : IFarmListParser
    {
        public HtmlNode GetStartButton(HtmlDocument doc, FarmListId raidId)
        {
            var farmNode = doc.GetElementbyId($"raidList{raidId}");
            if (farmNode is null) return null;
            var startNode = farmNode.Descendants("button")
                                    .FirstOrDefault(x => x.HasClass("startButton"));
            return startNode;
        }

        public HtmlNode GetStartAllButton(HtmlDocument doc)
        {
            var raidList = doc.GetElementbyId("raidList");
            if (raidList is null) return null;
            var startAll = raidList.Descendants("button").FirstOrDefault(x => x.HasClass("startAll"));
            return startAll;
        }
    }
}