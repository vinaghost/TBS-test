using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Parsers.FarmParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IFarmParser
    {
        public HtmlNode GetStartButton(HtmlDocument doc, FarmId raidId)
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