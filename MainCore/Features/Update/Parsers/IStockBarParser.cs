using HtmlAgilityPack;
using MainCore.Features.Update.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IStockBarParser
    {
        StorageDto Get(HtmlDocument doc);
    }
}