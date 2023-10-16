﻿using HtmlAgilityPack;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.InstantUpgrade.Parsers
{
    [RegisterAsTransient]
    public class InstantUpgradeParser : IInstantUpgradeParser
    {
        public HtmlNode GetCompleteButton(HtmlDocument doc)
        {
            var finishClass = doc.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishClass is null) return null;
            var button = finishClass
                .Descendants("button")
                .FirstOrDefault();
            return button;
        }

        public HtmlNode GetConfirmButton(HtmlDocument doc)
        {
            var dialog = doc.GetElementbyId("finishNowDialog");
            if (dialog is null) return null;
            var button = dialog
                .Descendants("button")
                .FirstOrDefault();
            return button;
        }
    }
}