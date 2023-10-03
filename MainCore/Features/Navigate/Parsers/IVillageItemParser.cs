using HtmlAgilityPack;

namespace MainCore.Features.Navigate.Parsers
{
    public interface IVillageItemParser
    {
        HtmlNode GetVillageNode(HtmlDocument doc, int villageId);
        bool IsActive(HtmlNode node);
    }
}