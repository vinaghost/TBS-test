using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IFarmListParser
    {
        IEnumerable<FarmListDto> Get(HtmlDocument doc);
    }
}