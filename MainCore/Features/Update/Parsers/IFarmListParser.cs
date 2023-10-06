using HtmlAgilityPack;
using MainCore.Features.Update.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IFarmListParser
    {
        IEnumerable<FarmListDto> Get(HtmlDocument doc);
    }
}