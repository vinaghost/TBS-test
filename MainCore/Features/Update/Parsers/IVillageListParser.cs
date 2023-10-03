using HtmlAgilityPack;

namespace MainCore.Features.Update.Parsers
{
    public interface IVillageListParser
    {
        int GetId(HtmlNode node);

        string GetName(HtmlNode node);

        List<HtmlNode> GetVillages(HtmlDocument doc);

        int GetX(HtmlNode node);

        int GetY(HtmlNode node);
        bool IsActive(HtmlNode node);
        bool IsUnderAttack(HtmlNode node);
    }
}