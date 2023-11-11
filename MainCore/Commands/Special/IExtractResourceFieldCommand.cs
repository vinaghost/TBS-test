using FluentResults;
using MainCore.DTO;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IExtractResourceFieldCommand
    {
        Task<Result> Execute(VillageId villageId, JobDto job);
    }
}