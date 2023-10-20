using FluentResults;

namespace MainCore.Common.Commands
{
    public interface IDelayCommand
    {
        Task<Result> Execute(int accountId);
    }
}