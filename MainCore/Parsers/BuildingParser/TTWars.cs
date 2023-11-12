using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Parsers.BuildingParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IBuildingParser
    {
        public HtmlNode GetBuilding(HtmlDocument doc, int location)
        {
            if (location < 19) return GetField(doc, location);
            return GetInfrastructure(doc, location);
        }

        private static HtmlNode GetField(HtmlDocument doc, int location)
        {
            var node = doc.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass($"buildingSlot{location}"));
            return node;
        }

        private static HtmlNode GetInfrastructure(HtmlDocument doc, int location)
        {
            var tmpLocation = location - 1;
            var node = doc.DocumentNode
                .SelectSingleNode($"//*[@id='village_map']/div[{tmpLocation}]");

            return node;
        }
    }
}