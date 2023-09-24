using FluentResults;
using MainCore.Enums;
using MainCore.Repositories;

namespace MainCore.Commands
{
    public class DelayCommand : IDelayCommand
    {
        private readonly IAccountSettingRepository _settingRepository;

        public DelayCommand(IAccountSettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<Result> Execute(int accountId)
        {
            var delay = await _settingRepository.GetSetting(accountId, AccountSettingEnums.ClickDelayMin, AccountSettingEnums.ClickDelayMax);
            await Task.Delay(delay);
            return Result.Ok();
        }
    }
}