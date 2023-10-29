using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
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
            var villageName = _context.Villages
                .Where(x => x.Id == villageId)
                .Select(x => x.Name)
                .FirstOrDefault();
            return villageName;
        }

        public async Task<VillageId> GetActiveVillageId(AccountId accountId)
        {
            var village = await Task.Run(() =>
                _context.Villages
                    .Where(x => x.AccountId == accountId && x.IsActive)
                    .Select(x => x.Id)
                    .FirstOrDefault());
            return village;
        }

        public async Task<List<VillageId>> GetInactiveVillageId(AccountId accountId)
        {
            var villages = await Task.Run(() =>
                _context.Villages
                    .Where(x => x.AccountId == accountId && !x.IsActive)
                    .OrderBy(x => x.Name)
                    .Select(x => x.Id)
                    .ToList());
            return villages;
        }

        public async Task<VillageDto> GetById(VillageId villageId)
        {
            var village = await Task.Run(() => _context.Villages.Find(villageId));
            var mapper = new VillageMapper();
            var dto = mapper.Map(village);
            return dto;
        }

        public async Task<List<VillageDto>> GetAll(AccountId accountId)
        {
            var dtos = await Task.Run(() =>
                _context.Villages
                    .Where(x => x.AccountId == accountId)
                    .OrderBy(x => x.Name)
                    .ProjectToDto()
                    .ToList());
            return dtos;
        }

        public async Task<List<VillageId>> GetUnloadVillageId(AccountId accountId)
        {
            var villages = await Task.Run(() =>
                _context.Villages
                    .Where(x => x.AccountId == accountId)
                    .Include(x => x.Buildings)
                    .Where(x => x.Buildings.Count < 19)
                    .OrderBy(x => x.Name)
                    .Select(x => x.Id)
                    .ToList());
            return villages;
        }
    }
}