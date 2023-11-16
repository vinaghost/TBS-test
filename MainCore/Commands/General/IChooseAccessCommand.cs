using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Commands.General
{
    public interface IChooseAccessCommand
    {
        Task<Result<AccessDto>> Execute(AccountId accountId, bool ignoreSleepTime);
    }
}