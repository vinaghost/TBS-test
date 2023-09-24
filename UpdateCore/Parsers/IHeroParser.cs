using HtmlAgilityPack;
using MainCore.Models;

namespace UpdateCore.Parsers
{
    public interface IHeroParser
    {
        List<HeroItem> GetItems(HtmlDocument doc);
    }
}