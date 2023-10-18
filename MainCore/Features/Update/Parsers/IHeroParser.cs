using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IHeroParser
    {
        IEnumerable<HeroItemDto> GetItems(HtmlDocument doc);
    }
}