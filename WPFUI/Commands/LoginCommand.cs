using LoginCore.Tasks;
using MainCore.Commands;
using MainCore.Enums;
using MainCore.Services;
using System.Threading.Tasks;
using WPFUI.Repositories;

namespace WPFUI.Commands
{
    public class LoginCommand : ILoginCommand
    {
        private readonly ITaskManager _taskManager;
        private readonly ITimerManager _timerManager;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IOpenBrowserCommand _openBrowserCommand;

        public LoginCommand(ITaskManager taskManager, IAccountSettingRepository accountSettingRepository, IOpenBrowserCommand openBrowserCommand, ITimerManager timerManager)
        {
            _taskManager = taskManager;
            _accountSettingRepository = accountSettingRepository;
            _openBrowserCommand = openBrowserCommand;
            _timerManager = timerManager;
        }

        public async Task Execute(int accountId)
        {
            _taskManager.SetStatus(accountId, StatusEnums.Starting);
            await _accountSettingRepository.CheckSetting(accountId);
            await _openBrowserCommand.Execute(accountId);

            _taskManager.Add<LoginTask>(accountId);
            _timerManager.Start(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
        }
    }
}