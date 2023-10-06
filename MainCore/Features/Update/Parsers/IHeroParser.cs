using HtmlAgilityPack;
using MainCore.Features.Update.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IHeroParser
    {
        IEnumerable<HeroItemDto> GetItems(HtmlDocument doc);
    }
}