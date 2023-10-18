﻿using HtmlAgilityPack;
using MainCore.DTO;

namespace MainCore.Features.Update.Parsers
{
    public interface IInfrastructureParser
    {
        IEnumerable<BuildingDto> Get(HtmlDocument doc);
    }
}