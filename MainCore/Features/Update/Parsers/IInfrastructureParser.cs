using HtmlAgilityPack;
using MainCore.Features.Update.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IInfrastructureParser
    {
        IEnumerable<BuildingDto> Get(HtmlDocument doc);
    }
}