﻿using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors.Storage;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class HeroItemRepository : IHeroItemRepository
    {
        private readonly AppDbContext _context;

        public HeroItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public Result IsEnoughResource(int accountId, long[] requiredResource)
        {
           
            var items = _context.HeroItems.Where(x => x.AccountId == accountId);

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

        public void Update(int accountId, IEnumerable<HeroItem> items)
        {
           
            var dbItems = _context.HeroItems.Where(x => x.AccountId == accountId).ToList();
            foreach (var item in items)
            {
                var dbItem = dbItems.FirstOrDefault(x => x.Type == item.Type);
                if (dbItem is null)
                {
                    _context.Add(item);
                }
                else
                {
                    dbItem.Type = item.Type;
                    dbItem.Amount = item.Amount;
                    _context.Update(dbItem);
                }
            }
            _context.SaveChanges();
        }
    }
}