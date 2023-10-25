using HtmlAgilityPack;
using MainCore.Entities;

namespace MainCore.Features.Farming.Parsers
{
    public interface IFarmListParser
    {
        HtmlNode GetStartAllButton(HtmlDocument doc);

        HtmlNode GetStartButton(HtmlDocument doc, FarmListId raidId);
    }
}