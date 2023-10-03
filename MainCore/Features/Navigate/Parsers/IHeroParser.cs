using HtmlAgilityPack;
using MainCore.Common.Enums;

namespace MainCore.Features.Navigate.Parsers
{
    public interface IHeroParser
    {
        HtmlNode GetAmountBox(HtmlDocument doc);

        HtmlNode GetConfirmButton(HtmlDocument doc);

        HtmlNode GetHeroAvatar(HtmlDocument doc);

        HtmlNode GetHeroTab(HtmlDocument doc, int index);

        HtmlNode GetItemSlot(HtmlDocument doc, HeroItemEnums type);

        bool IsCurrentTab(HtmlNode tabNode);
    }
}