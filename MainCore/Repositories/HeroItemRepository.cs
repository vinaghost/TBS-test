﻿using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors.Storage;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class HeroItemRepository : IHeroItemRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public HeroItemRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Result IsEnoughResource(AccountId accountId, long[] requiredResource)
        {
            using var context = _contextFactory.CreateDbContext();
            var types = new List<HeroItemEnums>() {
                HeroItemEnums.Wood,
                HeroItemEnums.Clay,
                HeroItemEnums.Iron,
                HeroItemEnums.Crop,
            };

            var items = context.HeroItems
                .Where(x => x.AccountId == accountId.Value)
                .Where(x => types.Contains(x.Type))
                .OrderBy(x => x.Type)
                .ToList();

            var result = Result.Ok();
            var wood = items.FirstOrDefault(x => x.Type == HeroItemEnums.Wood);
            var woodAmount = wood?.Amount ?? 0;
            if (woodAmount < requiredResource[0]) result.WithError(new Resource("wood", woodAmount, requiredResource[0]));
            var clay = items.FirstOrDefault(x => x.Type == HeroItemEnums.Clay);
            var clayAmount = clay?.Amount ?? 0;
            if (clayAmount < requiredResource[1]) result.WithError(new Resource("clay", clayAmount, requiredResource[1]));
            var iron = items.FirstOrDefault(x => x.Type == HeroItemEnums.Iron);
            var ironAmount = iron?.Amount ?? 0;
            if (ironAmount < requiredResource[2]) result.WithError(new Resource("iron", ironAmount, requiredResource[2]));
            var crop = items.FirstOrDefault(x => x.Type == HeroItemEnums.Crop);
            var cropAmount = crop?.Amount ?? 0;
            if (cropAmount < requiredResource[3]) result.WithError(new Resource("crop", cropAmount, requiredResource[3]));
            return result;
        }
    }
}