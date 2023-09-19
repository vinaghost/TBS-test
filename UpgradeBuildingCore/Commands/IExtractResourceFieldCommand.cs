using FluentResults;
using MainCore.Models;

namespace UpgradeBuildingCore.Commands
{
    public interface IExtractResourceFieldCommand
    {
        Task<Result> Execute(int villageId, Job job);
    }
}