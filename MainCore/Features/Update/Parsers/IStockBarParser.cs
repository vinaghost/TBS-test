using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IStockBarParser
    {
        StorageDto Get(HtmlDocument doc);
    }
}