using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IAccountInfoParser
    {
        AccountInfoDto Get(HtmlDocument doc);
    }
}