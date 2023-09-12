using HtmlAgilityPack;

namespace NavigateCore.Parsers
{
    public interface IVillageItemParser
    {
        HtmlNode GetVillageNode(HtmlDocument doc, int villageId);
        bool IsActive(HtmlNode node);
    }
}