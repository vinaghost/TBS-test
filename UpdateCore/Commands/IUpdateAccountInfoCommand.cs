using FluentResults;
using MainCore.Services;

namespace UpdateCore.Commands
{
    public interface IUpdateAccountInfoCommand
    {
        Task<Result> Execute(int accountId);
        Task<Result> Execute(int accountId, IChromeBrowser chromeBrowser);
    }
}