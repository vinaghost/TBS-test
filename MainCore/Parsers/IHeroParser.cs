using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.DTO;

namespace MainCore.Parsers
{
    public interface IHeroParser
    {
        HtmlNode GetAmountBox(HtmlDocument doc);

        HtmlNode GetConfirmButton(HtmlDocument doc);

        HtmlNode GetHeroAvatar(HtmlDocument doc);

        HtmlNode GetHeroTab(HtmlDocument doc, int index);

        HtmlNode GetItemSlot(HtmlDocument doc, HeroItemEnums type);

        bool IsCurrentTab(HtmlNode tabNode);

        IEnumerable<HeroItemDto> Get(HtmlDocument doc);
        bool HeroInventoryLoading(HtmlDocument doc);
    }
}