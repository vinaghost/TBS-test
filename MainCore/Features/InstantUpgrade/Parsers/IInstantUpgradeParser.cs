using HtmlAgilityPack;

namespace MainCore.Features.InstantUpgrade.Parsers
{
    public interface IInstantUpgradeParser
    {
        HtmlNode GetCompleteButton(HtmlDocument doc);
        HtmlNode GetConfirmButton(HtmlDocument doc);
    }
}