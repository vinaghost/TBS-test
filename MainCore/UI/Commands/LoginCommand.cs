using MainCore.Common.Commands;
using MainCore.Common.Enums;
using MainCore.Common.Repositories;
using MainCore.Features.Login.Tasks;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.UI.Commands
{
    [RegisterAsTransient]
    public class LoginCommand : ILoginCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly ITimerManager _timerManager;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IVillageRepository _villageRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IMediator _mediator;

        public LoginCommand(ITaskManager taskManager, IAccountSettingRepository accountSettingRepository, ITimerManager timerManager, IVillageRepository villageRepository, IJobRepository jobRepository, IMediator mediator)
        {
            _taskManager = taskManager;
            _accountSettingRepository = accountSettingRepository;
            _timerManager = timerManager;
            _villageRepository = villageRepository;
            _jobRepository = jobRepository;
            _mediator = mediator;
        }

        public async Task Execute(int accountId)
        {
            _taskManager.SetStatus(accountId, StatusEnums.Starting);
            await _mediator.Send(new OpenBrowserCommand(accountId));

            _taskManager.Add<LoginTask>(accountId, first: true);
            AddUpgradeBuildingTask(accountId);
            _timerManager.Start(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
        }

        private void AddUpgradeBuildingTask(int accountId)
        {
            var villages = _villageRepository.GetList(accountId);
            var now = DateTime.Now;
            var flag = false;
            foreach (var village in villages)
            {
                var countJob = _jobRepository.CountBuildingJob(village.Id);
                if (countJob > 0)
                {
                    flag = true;
                    var task = _taskManager.Get<UpgradeBuildingTask>(accountId, village.Id);
                    if (task is null)
                    {
                        _taskManager.Add<UpgradeBuildingTask>(accountId, village.Id);
                    }
                    else
                    {
                        task.ExecuteAt = now;
                    }
                }
            }
            if (flag)
            {
                _taskManager.ReOrder(accountId);
            }
        }
    }
}