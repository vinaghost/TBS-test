using HtmlAgilityPack;

namespace MainCore.Features.Navigate.Parsers
{
    public interface INavigationTabParser
    {
        int CountTab(HtmlDocument doc);
        HtmlNode GetTab(HtmlDocument doc, int index);
        bool IsTabActive(HtmlNode node);
    }
}