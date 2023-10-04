using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Navigate.Parsers.VillageItemParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IVillageItemParser
    {
        public HtmlNode GetVillageNode(HtmlDocument doc, int villageId)
        {
            var villageBox = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.Id.Equals("sidebarBoxVillagelist"));
            if (villageBox is null) return null;
            var villages = villageBox
                            .Descendants("li")
                            .ToList();
            var village = villages.FirstOrDefault(x => GetId(x) == villageId);
            return village;
        }

        public bool IsActive(HtmlNode node)
        {
            return node.HasClass("active");
        }

        private static int GetId(HtmlNode node)
        {
            var dataDid = node.GetAttributeValue("data-did", 0);
            return dataDid;
        }
    }
}