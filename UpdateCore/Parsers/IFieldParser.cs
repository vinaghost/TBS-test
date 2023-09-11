using HtmlAgilityPack;
using MainCore.Enums;

namespace UpdateCore.Parsers
{
    public interface IFieldParser
    {
        BuildingEnums GetBuildingType(HtmlNode node);

        int GetId(HtmlNode node);

        int GetLevel(HtmlNode node);

        HtmlNode GetNode(HtmlDocument doc, int index);

        List<HtmlNode> GetNodes(HtmlDocument doc);

        bool IsUnderConstruction(HtmlNode node);
    }
}