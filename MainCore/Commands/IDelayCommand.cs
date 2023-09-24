using FluentResults;

namespace MainCore.Commands
{
    public interface IDelayCommand
    {
        Task<Result> Execute(int accountId);
    }
}