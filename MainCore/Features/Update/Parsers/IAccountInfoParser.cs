using HtmlAgilityPack;
using MainCore.Features.Update.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IAccountInfoParser
    {
        AccountInfoDto Get(HtmlDocument doc);
    }
}