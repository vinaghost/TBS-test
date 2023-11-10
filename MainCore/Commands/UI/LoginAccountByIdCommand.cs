using FluentResults;
using MainCore.Commands.Base;
using MainCore.Common;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Features.Login.Tasks;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Commands.UI
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
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfWork _unitOfWork;

        public LoginAccountByIdCommandHandler(ITaskManager taskManager, ITimerManager timerManager, IChromeManager chromeManager, IUnitOfWork unitOfWork)
        {
            _taskManager = taskManager;
            _timerManager = timerManager;
            _chromeManager = chromeManager;
            _unitOfWork = unitOfWork;
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

            var account = _unitOfWork.AccountRepository.Get(accountId);
            var access = _unitOfWork.AccountRepository.GetAccess(accountId);

            var result = chromeBrowser.Setup(account, access);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        private void AddUpgradeBuildingTask(AccountId accountId)
        {
            var hasBuildingJobVillages = _unitOfWork.VillageRepository.GetHasBuildingJobVillages(accountId);
            foreach (var village in hasBuildingJobVillages)
            {
                _taskManager.AddOrUpdate<UpgradeBuildingTask>(accountId, village);
            }
        }
    }
}