using FluentResults;
using MainCore.Common;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Commands.General
{
    [RegisterAsTransient]
    public class DelayClickCommand : IDelayClickCommand
    {
        private readonly IUnitOfWork _unitOfWork;

        public DelayClickCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var delay = _unitOfWork.AccountSettingRepository.GetByName(accountId, AccountSettingEnums.ClickDelayMin, AccountSettingEnums.ClickDelayMax);
            await Task.Delay(delay);
            return Result.Ok();
        }
    }
}