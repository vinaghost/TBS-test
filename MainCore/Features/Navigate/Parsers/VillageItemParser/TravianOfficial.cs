using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Navigate.Parsers.VillageItemParser
{
    [RegisterAsTransient(ServerEnums.TravianOfficial)]
    public class TravianOfficial : IVillageItemParser
    {
        public HtmlNode GetVillageNode(HtmlDocument doc, VillageId villageId)
        {
            var villageBox = doc.GetElementbyId("sidebarBoxVillagelist");
            if (villageBox is null) return null;
            var villages = villageBox
                            .Descendants("div")
                            .Where(x => x.HasClass("listEntry"))
                            .ToList();
            var village = villages.FirstOrDefault(x => GetId(x) == villageId);
            return village;
        }

        public bool IsActive(HtmlNode node)
        {
            return node.HasClass("active");
        }

        private static VillageId GetId(HtmlNode node)
        {
            var dataDid = node.GetAttributeValue("data-did", 0);
            return new VillageId(dataDid);
        }
    }
}