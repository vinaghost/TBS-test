using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Update.Parsers.HeroParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IHeroParser
    {
        public List<HeroItem> GetItems(HtmlDocument doc)
        {
            var heroItems = new List<HeroItem>();
            var inventory = doc.GetElementbyId("itemsToSale");
            if (inventory is null) return null;

            foreach (var itemSlot in inventory.ChildNodes)
            {
                var item = itemSlot.ChildNodes.FirstOrDefault(x => x.Id.StartsWith("item_"));
                if (item is null) continue;

                var itemClass = item.GetClasses().FirstOrDefault(x => x.Contains("_item_"));
                var itemValue = itemClass.Split('_').LastOrDefault();
                if (itemValue is null) continue;

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                var amountValue = item.ChildNodes.FirstOrDefault(x => x.HasClass("amount"));
                if (amountValue is null)
                {
                    heroItems.Add(new HeroItem()
                    {
                        Id = int.Parse(itemValueStr),
                        Amount = 1,
                    });
                    continue;
                }

                var amountValueStr = new string(amountValue.InnerText.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr))
                {
                    heroItems.Add(new HeroItem()
                    {
                        Id = int.Parse(itemValueStr),
                        Amount = 1,
                    });
                    continue;
                }
                heroItems.Add(new HeroItem()
                {
                    Id = int.Parse(itemValueStr),
                    Amount = int.Parse(amountValueStr),
                });
            }
            return heroItems;
        }
    }
}