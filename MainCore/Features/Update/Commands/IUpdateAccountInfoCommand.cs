using FluentResults;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    public interface IUpdateAccountInfoCommand
    {
        Task<Result> Execute(int accountId);
        Task<Result> Execute(int accountId, IChromeBrowser chromeBrowser);
    }
}