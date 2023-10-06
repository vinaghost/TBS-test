using HtmlAgilityPack;

namespace MainCore.Features.Navigate.Parsers.InfrastructureParser
{
    public class TravianOfficial : IInfrastructureParser
    {
        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            var location = index - 18; // - 19 + 1
            return doc.DocumentNode.SelectSingleNode($"//*[@id='villageContent']/div[{location}]");
        }
    }
}