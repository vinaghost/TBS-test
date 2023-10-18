using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IQueueBuildingParser
    {
        IEnumerable<QueueBuildingDto> Get(HtmlDocument doc);
    }
}