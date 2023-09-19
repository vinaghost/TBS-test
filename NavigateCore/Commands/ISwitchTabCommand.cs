using FluentResults;

namespace NavigateCore.Commands
{
    public interface ISwitchTabCommand
    {
        Task<Result> Execute(int accountId, int index);
    }
}