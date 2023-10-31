using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Farming.Commands
{
    public interface ISendAllFarmListCommand
    {
        Result Execute(AccountId accountId);
    }
}