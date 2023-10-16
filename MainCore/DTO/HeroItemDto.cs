﻿using MainCore.Common.Enums;
using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.Features.Update.DTO
{
    public class HeroItemDto
    {
        public HeroItemEnums Type { get; set; }
        public int Amount { get; set; }
    }

    [Mapper]
    public partial class HeroItemMapper
    {
        public HeroItem Map(int accountId, HeroItemDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        private partial HeroItem Map(HeroItemDto dto);
    }
}