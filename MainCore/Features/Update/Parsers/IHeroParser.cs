using HtmlAgilityPack;
using MainCore.Entities;

namespace MainCore.Features.Update.Parsers
{
    public interface IHeroParser
    {
        List<HeroItem> GetItems(HtmlDocument doc);
    }
}