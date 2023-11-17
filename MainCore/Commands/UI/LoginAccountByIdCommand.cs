using FluentResults;
using MainCore.Commands.Base;
using MainCore.Commands.General;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
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
        private readonly IUnitOfRepository _unitOfRepository;

        private readonly IChooseAccessCommand _chooseAccessCommand;
        private readonly IWorkCommand _workCommand;
        private readonly ILogService _logService;

        public LoginAccountByIdCommandHandler(ITaskManager taskManager, ITimerManager timerManager, IChromeManager chromeManager, IUnitOfRepository unitOfRepository, IWorkCommand workCommand, IChooseAccessCommand chooseAccessCommand, ILogService logService)
        {
            _taskManager = taskManager;
            _timerManager = timerManager;
            _chromeManager = chromeManager;
            _unitOfRepository = unitOfRepository;
            _workCommand = workCommand;
            _chooseAccessCommand = chooseAccessCommand;
            _logService = logService;
        }

        public async Task<Result> Handle(LoginAccountByIdCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            _taskManager.SetStatus(accountId, StatusEnums.Starting);

            var resultAccessChooser = await _chooseAccessCommand.Execute(accountId, true);
            if (resultAccessChooser.IsFailed) return Result.Fail(resultAccessChooser.Errors).WithError(new TraceMessage(TraceMessage.Line()));
            var logger = _logService.GetLogger(accountId);
            var access = resultAccessChooser.Value;
            logger.Information("Using connection {proxy} to start chrome", access.Proxy);
            var result = await Task.Run(() => _workCommand.Execute(accountId, access), cancellationToken);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            _taskManager.AddOrUpdate<LoginTask>(accountId, first: true);
            await Task.Run(() => AddUpgradeBuildingTask(accountId), cancellationToken);
            AddSleepTask(accountId);

            _timerManager.Start(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
            return Result.Ok();
        }

        private void AddUpgradeBuildingTask(AccountId accountId)
        {
            var hasBuildingJobVillages = _unitOfRepository.VillageRepository.GetHasBuildingJobVillages(accountId);
            foreach (var village in hasBuildingJobVillages)
            {
                _taskManager.AddOrUpdate<UpgradeBuildingTask>(accountId, village);
            }
        }

        private void AddSleepTask(AccountId accountId)
        {
            var workTime = _unitOfRepository.AccountSettingRepository.GetByName(accountId, AccountSettingEnums.WorkTimeMin, AccountSettingEnums.WorkTimeMax);
            _taskManager.AddOrUpdate<SleepTask>(accountId, executeTime: DateTime.Now.AddMinutes(workTime));
        }
    }
}