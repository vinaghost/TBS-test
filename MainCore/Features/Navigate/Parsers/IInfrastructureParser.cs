using HtmlAgilityPack;

namespace MainCore.Features.Navigate.Parsers
{
    public interface IInfrastructureParser
    {
        HtmlNode GetNode(HtmlDocument doc, int index);
    }
}