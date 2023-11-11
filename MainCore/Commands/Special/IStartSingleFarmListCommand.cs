using FluentResults;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IStartSingleFarmListCommand
    {
        Result Execute(AccountId accountId, FarmId farmlistId);
    }
}