using FluentResults;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;

namespace MainCore.Common.Commands
{
    [RegisterAsTransient]
    public class DelayClickCommand : IDelayClickCommand
    {
        private readonly IAccountSettingRepository _accountSettingRepository;

        public DelayClickCommand(IAccountSettingRepository accountSettingRepository)
        {
            _accountSettingRepository = accountSettingRepository;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var delay = _accountSettingRepository.GetByName(accountId, AccountSettingEnums.ClickDelayMin, AccountSettingEnums.ClickDelayMax);
            await Task.Delay(delay);
            return Result.Ok();
        }
    }
}