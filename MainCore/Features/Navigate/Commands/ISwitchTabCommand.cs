using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface ISwitchTabCommand
    {
        Result Execute(AccountId accountId, int index);
    }
}