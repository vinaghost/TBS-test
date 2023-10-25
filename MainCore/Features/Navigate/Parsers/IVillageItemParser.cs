using HtmlAgilityPack;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Parsers
{
    public interface IVillageItemParser
    {
        HtmlNode GetVillageNode(HtmlDocument doc, VillageId villageId);

        bool IsActive(HtmlNode node);
    }
}