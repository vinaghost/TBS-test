using FluentResults;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    public interface IUpdateDorfCommand
    {
        Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId);
        Task<Result> Execute(int accountId, int villageId);
    }
}