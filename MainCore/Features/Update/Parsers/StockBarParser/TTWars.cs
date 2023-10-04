using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using System.Net;

namespace MainCore.Features.Update.Parsers.StockBarParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IStockBarParser
    {
        public Storage GetStorage(HtmlDocument doc)
        {
            var storage = new Storage()
            {
                Wood = GetWood(doc),
                Clay = GetClay(doc),
                Iron = GetIron(doc),
                Crop = GetCrop(doc),
                FreeCrop = GetFreeCrop(doc),
                Warehouse = GetWarehouseCapacity(doc),
                Granary = GetGranaryCapacity(doc)
            };
            return storage;
        }

        private static long GetResource(HtmlDocument doc, string id)
        {
            var node = doc.GetElementbyId(id);
            if (node is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(node.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public long GetWood(HtmlDocument doc) => GetResource(doc, "l1");

        public long GetClay(HtmlDocument doc) => GetResource(doc, "l2");

        public long GetIron(HtmlDocument doc) => GetResource(doc, "l3");

        public long GetCrop(HtmlDocument doc) => GetResource(doc, "l4");

        public long GetFreeCrop(HtmlDocument doc) => GetResource(doc, "stockBarFreeCrop");

        public long GetWarehouseCapacity(HtmlDocument doc)
        {
            var stockBarNode = doc.GetElementbyId("stockBar");
            if (stockBarNode is null) return -1;
            var warehouseNode = stockBarNode.Descendants("div").FirstOrDefault(x => x.HasClass("warehouse"));
            if (warehouseNode is null) return -1;
            var capacityNode = warehouseNode.Descendants("div").FirstOrDefault(x => x.HasClass("capacity"));
            if (capacityNode is null) return -1;
            var valueNode = capacityNode.Descendants("div").FirstOrDefault(x => x.HasClass("value"));
            if (valueNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(valueNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public long GetGranaryCapacity(HtmlDocument doc)
        {
            var stockBarNode = doc.GetElementbyId("stockBar");
            if (stockBarNode is null) return -1;
            var granaryNode = stockBarNode.Descendants("div").FirstOrDefault(x => x.HasClass("granary"));
            if (granaryNode is null) return -1;
            var capacityNode = granaryNode.Descendants("div").FirstOrDefault(x => x.HasClass("capacity"));
            if (capacityNode is null) return -1;
            var valueNode = capacityNode.Descendants("div").FirstOrDefault(x => x.HasClass("value"));
            if (valueNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(valueNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public int GetGold(HtmlDocument doc)
        {
            var goldNode = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("ajaxReplaceableGoldAmount"));
            if (goldNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(goldNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return int.Parse(valueStr);
        }

        public int GetSilver(HtmlDocument doc)
        {
            var silverNode = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("ajaxReplaceableSilverAmount"));
            if (silverNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(silverNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return int.Parse(valueStr);
        }
    }
}