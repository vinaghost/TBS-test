using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Navigate.Parsers.VillageItemParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IVillageItemParser
    {
        public HtmlNode GetVillageNode(HtmlDocument doc, VillageId villageId)
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

        private static VillageId GetId(HtmlNode node)
        {
            var hrefNode = node.Descendants("a").FirstOrDefault();
            if (hrefNode is null) return VillageId.Empty;
            var href = System.Net.WebUtility.HtmlDecode(hrefNode.GetAttributeValue("href", ""));
            if (string.IsNullOrEmpty(href)) return VillageId.Empty;
            if (!href.Contains('=')) return VillageId.Empty;
            var value = href.Split('=')[1];
            if (value.Contains('&'))
            {
                value = value.Split('&')[0];
            }
            return new VillageId(int.Parse(value));
        }
    }
}