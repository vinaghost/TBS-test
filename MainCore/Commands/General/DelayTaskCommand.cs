using FluentResults;
using MainCore.Common;
using MainCore.Common.Enums;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Commands.General
{
    [RegisterAsTransient]
    public class DelayTaskCommand : IDelayTaskCommand
    {
        private readonly IUnitOfWork _unitOfWork;

        public DelayTaskCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var delay = _unitOfWork.AccountSettingRepository.GetByName(accountId, AccountSettingEnums.TaskDelayMin, AccountSettingEnums.TaskDelayMax);
            await Task.Delay(delay);
            return Result.Ok();
        }
    }
}