using FluentResults;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IAddCroplandCommand
    {
        Task<Result> Execute(VillageId villageId);
    }
}