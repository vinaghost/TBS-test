using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Navigate.Parsers.HeroParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IHeroParser
    {
        public HtmlNode GetHeroTab(HtmlDocument doc, int index)
        {
            var heroDiv = doc.GetElementbyId("content");
            if (heroDiv is null) return null;
            var aNode = heroDiv.Descendants("a").ToList();
            if (index >= aNode.Count) return null;
            return aNode[index];
        }

        public bool IsCurrentTab(HtmlNode tabNode)
        {
            return tabNode.HasClass("active");
        }

        public HtmlNode GetHeroAvatar(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroImageButton");
        }

        public HtmlNode GetItemSlot(HtmlDocument doc, HeroItemEnums type)
        {
            var inventory = doc.GetElementbyId("itemsToSale");
            foreach (var itemSlot in inventory.ChildNodes)
            {
                var item = itemSlot.ChildNodes.FirstOrDefault(x => x.Id.StartsWith("item_"));
                if (item is null) continue;

                var itemClass = item.GetClasses().FirstOrDefault(x => x.Contains("_item_"));
                var itemValue = itemClass.Split('_').LastOrDefault();
                if (itemValue is null) continue;

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                if (int.Parse(itemValueStr) == (int)type) return item;
            }
            return null;
        }

        public HtmlNode GetAmountBox(HtmlDocument doc)
        {
            return doc.GetElementbyId("amount");
        }

        public HtmlNode GetConfirmButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("ok"));
        }
    }
}