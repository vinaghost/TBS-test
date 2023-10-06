using HtmlAgilityPack;

namespace MainCore.Features.Navigate.Parsers.InfrastructureParser
{
    public interface IInfrastructureParser
    {
        HtmlNode GetNode(HtmlDocument doc, int index);
    }
}