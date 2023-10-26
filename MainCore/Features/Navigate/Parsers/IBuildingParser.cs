using HtmlAgilityPack;

namespace MainCore.Features.Navigate.Parsers
{
    public interface IBuildingParser
    {
        HtmlNode GetBuilding(HtmlDocument doc, int location);
    }
}