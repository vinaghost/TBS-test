using HtmlAgilityPack;
using MainCore.Features.Update.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IQueueBuildingParser
    {
        IEnumerable<QueueBuildingDto> Get(HtmlDocument doc);
    }
}