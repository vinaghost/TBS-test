using HtmlAgilityPack;
using MainCore.Features.Update.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IVillageListParser
    {
        IEnumerable<VillageDto> Get(HtmlDocument doc);
    }
}