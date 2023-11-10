using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IFarmListParser
    {
        IEnumerable<FarmDto> Get(HtmlDocument doc);
    }
}