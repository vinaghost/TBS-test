﻿using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class VillageRepository : IVillageRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public VillageRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public string GetVillageName(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villageName = context.Villages
                .Where(x => x.Id == villageId)
                .Select(x => x.Name)
                .FirstOrDefault();
            return villageName;
        }

        public VillageId GetActiveVillageId(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var village = context.Villages
                    .Where(x => x.AccountId == accountId)
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            return village;
        }

        public List<VillageId> GetInactiveVillageId(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages
                    .Where(x => x.AccountId == accountId)
                    .Where(x => !x.IsActive)
                    .OrderBy(x => x.Name)
                    .Select(x => x.Id)
                    .ToList();
            return villages;
        }
    }
}