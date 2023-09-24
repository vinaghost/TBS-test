using HtmlAgilityPack;

namespace UpdateCore.Parsers
{
    public interface IAccountInfoParser
    {
        int GetGold(HtmlDocument doc);
        int GetSilver(HtmlDocument doc);
        bool HasPlusAccount(HtmlDocument doc);
    }
}