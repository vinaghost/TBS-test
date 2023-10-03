using HtmlAgilityPack;
using MainCore.Common.Enums;

namespace MainCore.Features.Update.Parsers
{
    public interface IInfrastructureParser
    {
        BuildingEnums GetBuildingType(HtmlNode node);

        int GetId(HtmlNode node);

        int GetLevel(HtmlNode node);

        HtmlNode GetNode(HtmlDocument doc, int index);

        List<HtmlNode> GetNodes(HtmlDocument doc);

        bool IsUnderConstruction(HtmlNode node);
    }
}