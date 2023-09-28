using LoginCore.Tasks;
using MainCore.Commands;
using MainCore.Enums;
using MainCore.Repositories;
using MainCore.Services;
using System;
using System.Threading.Tasks;
using UpgradeBuildingCore.Tasks;
using IAccountSettingRepository = WPFUI.Repositories.IAccountSettingRepository;

namespace WPFUI.Commands
{
    public class LoginCommand : ILoginCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly ITimerManager _timerManager;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IOpenBrowserCommand _openBrowserCommand;
        private readonly IVillageRepository _villageRepository;
        private readonly IJobRepository _jobRepository;

        public LoginCommand(ITaskManager taskManager, IAccountSettingRepository accountSettingRepository, IOpenBrowserCommand openBrowserCommand, ITimerManager timerManager, IVillageRepository villageRepository, IJobRepository jobRepository)
        {
            _taskManager = taskManager;
            _accountSettingRepository = accountSettingRepository;
            _openBrowserCommand = openBrowserCommand;
            _timerManager = timerManager;
            _villageRepository = villageRepository;
            _jobRepository = jobRepository;
        }

        public async Task Execute(int accountId)
        {
            _taskManager.SetStatus(accountId, StatusEnums.Starting);
            await _openBrowserCommand.Execute(accountId);

            _taskManager.Add<LoginTask>(accountId, first: true);
            await AddUpgradeBuildingTask(accountId);
            _timerManager.Start(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
        }

        private async Task AddUpgradeBuildingTask(int accountId)
        {
            var villages = await _villageRepository.GetList(accountId);
            var now = DateTime.Now;
            var flag = false;
            foreach (var village in villages)
            {
                var countJob = await _jobRepository.CountBuildingJob(village.Id);
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