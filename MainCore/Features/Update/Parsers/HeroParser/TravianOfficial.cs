﻿using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Features.Update.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Update.Parsers.HeroParser
{
    [RegisterAsTransient(ServerEnums.TravianOfficial)]
    public class TravianOfficial : IHeroParser
    {
        public IEnumerable<HeroItemDto> GetItems(HtmlDocument doc)
        {
            var heroItemsDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroItems"));
            if (heroItemsDiv is null) yield break;
            var heroItemDivs = heroItemsDiv.Descendants("div").Where(x => x.HasClass("heroItem") && !x.HasClass("empty"));
            if (!heroItemDivs.Any()) yield break;

            foreach (var itemSlot in heroItemDivs)
            {
                if (itemSlot.ChildNodes.Count < 2) continue;
                var itemNode = itemSlot.ChildNodes[1];
                var classes = itemNode.GetClasses();
                if (classes.Count() != 2) continue;

                var itemValue = classes.ElementAt(1);
                if (itemValue is null) continue;

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                if (!itemSlot.GetAttributeValue("data-tier", "").Contains("consumable"))
                {
                    yield return new HeroItemDto()
                    {
                        Type = (HeroItemEnums)int.Parse(itemValueStr),
                        Amount = 1,
                    };
                    continue;
                }

                if (itemSlot.ChildNodes.Count < 3)
                {
                    yield return new HeroItemDto()
                    {
                        Type = (HeroItemEnums)int.Parse(itemValueStr),
                        Amount = 1,
                    };
                    continue;
                }
                var amountNode = itemSlot.ChildNodes[2];

                var amountValueStr = new string(amountNode.InnerText.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(amountValueStr))
                {
                    yield return new HeroItemDto()
                    {
                        Type = (HeroItemEnums)int.Parse(itemValueStr),
                        Amount = 1,
                    };
                    continue;
                }
                yield return new HeroItemDto()
                {
                    Type = (HeroItemEnums)int.Parse(itemValueStr),
                    Amount = int.Parse(amountValueStr),
                };
                continue;
            }
        }
    }
}