using FluentResults;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Commands
{
    [RegisterAsTransient]
    public class DelayClickCommand : IDelayClickCommand
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DelayClickCommand(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var delay = GetDelay(accountId);
            await Task.Delay(delay);
            return Result.Ok();
        }

        public int GetDelay(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = new List<AccountSettingEnums>() {
                AccountSettingEnums.ClickDelayMin,
                AccountSettingEnums.ClickDelayMax,
            };
            var valueSettings = context.AccountsSetting
                .AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .Where(x => settings.Contains(x.Setting))
                .Select(x => x.Value)
                .ToList();
            var min = valueSettings.Min();
            var max = valueSettings.Max();
            var value = Random.Shared.Next(min, max);
            return value;
        }
    }
}