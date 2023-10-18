using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IFieldParser
    {
        IEnumerable<BuildingDto> Get(HtmlDocument doc);
    }
}