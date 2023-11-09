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
                .AsNoTracking()
                .Where(x => x.Id == villageId.Value)
                .Select(x => x.Name)
                .FirstOrDefault();
            return villageName;
        }

        public VillageId GetActiveVillageId(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var village = context.Villages
                .AsNoTracking()
                .Where(x => x.AccountId == accountId.Value)
                .Where(x => x.IsActive)
                .Select(x => x.Id)
                .AsEnumerable()
                .Select(x => new VillageId(x))
                .FirstOrDefault();
            return village;
        }

        public List<VillageId> GetInactiveVillageId(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages
                .AsNoTracking()
                .Where(x => x.AccountId == accountId.Value)
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => x.Id)
                .AsEnumerable()
                .Select(x => new VillageId(x))
                .ToList();
            return villages;
        }

        public List<VillageId> GetMissingBuildingVillages(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var missingBuildingVillages = context.Villages
                .AsNoTracking()
                .Where(x => x.AccountId == accountId.Value)
                .Include(x => x.Buildings)
                .Where(x => x.Buildings.Count != 40)
                .Select(x => x.Id)
                .AsEnumerable()
                .Select(x => new VillageId(x))
                .ToList();
            return missingBuildingVillages;
        }
    }
}