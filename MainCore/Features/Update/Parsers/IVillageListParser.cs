using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IVillageListParser
    {
        IEnumerable<VillageDto> Get(HtmlDocument doc);
    }
}