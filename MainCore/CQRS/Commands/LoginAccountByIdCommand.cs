using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.CQRS.Base;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Features.Login.Tasks;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Commands
{
    public class LoginAccountByIdCommand : ByAccountIdRequestBase, IRequest<Result>
    {
        public LoginAccountByIdCommand(AccountId accountId) : base(accountId)
        {
        }
    }

    public class LoginAccountByIdCommandHandler : IRequestHandler<LoginAccountByIdCommand, Result>
    {
        private readonly ITaskManager _taskManager;
        private readonly ITimerManager _timerManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IChromeManager _chromeManager;

        public LoginAccountByIdCommandHandler(ITaskManager taskManager, ITimerManager timerManager, IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory)
        {
            _taskManager = taskManager;
            _timerManager = timerManager;
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
        }

        public async Task<Result> Handle(LoginAccountByIdCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            _taskManager.SetStatus(accountId, StatusEnums.Starting);

            var result = await Task.Run(() => SetupBrowser(accountId), cancellationToken);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            _taskManager.AddOrUpdate<LoginTask>(accountId, first: true);
            await Task.Run(() => AddUpgradeBuildingTask(accountId), cancellationToken);

            _timerManager.Start(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
            return Result.Ok();
        }

        private Result SetupBrowser(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts
                .AsNoTracking()
                .Where(x => x.Id == accountId.Value)
                .ToDto()
                .FirstOrDefault();

            var access = context.Accesses
                .AsNoTracking()
                .Where(x => x.AccountId == accountId.Value)
                .OrderBy(x => x.LastUsed) // get oldest one
                .ToDto()
                .FirstOrDefault();

            var result = chromeBrowser.Setup(account, access);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        private void AddUpgradeBuildingTask(AccountId accountId)
        {
            using var context = _contextFactory.CreateDbContext();

            var types = new List<JobTypeEnums>() {
                JobTypeEnums.NormalBuild,
                JobTypeEnums.ResourceBuild
            };
            var hasBuildJobVillages = context.Villages
                .AsNoTracking()
                .Where(x => x.AccountId == accountId.Value)
                .Select(x => x.Id)
                .Join(
                    context.Jobs,
                    villageId => villageId,
                    job => job.VillageId,
                    (villageId, job) => new
                    {
                        VillageId = villageId,
                        job.Type
                    })
                .Where(x => types.Contains(x.Type))
                .GroupBy(x => x.VillageId)
                .Select(x => x.Key)
                .AsEnumerable()
                .Select(x => new VillageId(x));

            foreach (var village in hasBuildJobVillages)
            {
                _taskManager.AddOrUpdate<UpgradeBuildingTask>(accountId, village);
            }
        }
    }
}