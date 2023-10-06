using HtmlAgilityPack;

namespace MainCore.Features.Navigate.Parsers
{
    public interface IFieldParser
    {
        HtmlNode GetNode(HtmlDocument doc, int index);
    }
}