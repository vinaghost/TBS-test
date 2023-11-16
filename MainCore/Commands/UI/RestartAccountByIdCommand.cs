using MainCore.Commands.Base;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.Services;
using MainCore.Repositories;
using MainCore.Tasks;
using MediatR;

namespace MainCore.Commands.UI
{
    public class RestartAccountByIdCommand : ByAccountIdRequestBase, IRequest
    {
        public RestartAccountByIdCommand(AccountId accountId) : base(accountId)
        {
        }
    }

    public class RestartCommandHandler : IRequestHandler<RestartAccountByIdCommand>
    {
        private readonly ITaskManager _taskManager;
        private readonly IDialogService _dialogService;
        private readonly IUnitOfRepository _unitOfRepository;

        public RestartCommandHandler(ITaskManager taskManager, IDialogService dialogService, IUnitOfRepository unitOfRepository)
        {
            _taskManager = taskManager;
            _dialogService = dialogService;
            _unitOfRepository = unitOfRepository;
        }

        public async Task Handle(RestartAccountByIdCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var status = _taskManager.GetStatus(accountId);

            switch (status)
            {
                case StatusEnums.Offline:
                case StatusEnums.Starting:
                case StatusEnums.Pausing:
                case StatusEnums.Stopping:
                    _dialogService.ShowMessageBox("Information", $"Account is {status}");
                    return;

                case StatusEnums.Online:
                    _dialogService.ShowMessageBox("Information", $"Account should be paused first");
                    return;

                case StatusEnums.Paused:
                    await Task.Run(() => Handle(accountId), cancellationToken);
                    return;
            }
        }

        private void Handle(AccountId accountId)
        {
            _taskManager.SetStatus(accountId, StatusEnums.Starting);
            _taskManager.Clear(accountId);
            _taskManager.AddOrUpdate<LoginTask>(accountId, first: true);
            AddUpgradeBuildingTask(accountId);
            AddSleepTask(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
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