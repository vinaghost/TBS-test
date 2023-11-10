using HtmlAgilityPack;
using MainCore.Common.Enums;

namespace MainCore.Parsers
{
    public interface ITroopPageParser
    {
        HtmlNode GetInputBox(HtmlNode node);

        int GetMaxAmount(HtmlNode node);

        HtmlNode GetNode(HtmlDocument doc, TroopEnums troop);

        TimeSpan GetQueueTrainTime(HtmlDocument doc);

        HtmlNode GetTrainButton(HtmlDocument doc);

        long[] GetTrainCost(HtmlNode node);

        TimeSpan GetTrainTime(HtmlNode node);
    }
}