using HtmlAgilityPack;
using MainCore.Entities;

namespace MainCore.Features.Update.Parsers
{
    public interface IStockBarParser
    {
        long GetClay(HtmlDocument doc);

        long GetCrop(HtmlDocument doc);

        long GetFreeCrop(HtmlDocument doc);

        int GetGold(HtmlDocument doc);

        long GetGranaryCapacity(HtmlDocument doc);

        long GetIron(HtmlDocument doc);

        int GetSilver(HtmlDocument doc);

        Storage GetStorage(HtmlDocument doc);

        long GetWarehouseCapacity(HtmlDocument doc);

        long GetWood(HtmlDocument doc);
    }
}