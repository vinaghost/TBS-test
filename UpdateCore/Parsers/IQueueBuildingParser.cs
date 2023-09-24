using HtmlAgilityPack;

namespace UpdateCore.Parsers
{
    public interface IQueueBuildingParser
    {
        string GetBuildingType(HtmlNode node);
        TimeSpan GetDuration(HtmlNode node);
        List<HtmlNode> GetNodes(HtmlDocument doc);
        int GetLevel(HtmlNode node);
    }
}