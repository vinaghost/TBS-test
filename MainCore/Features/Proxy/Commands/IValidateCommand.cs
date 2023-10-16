using FluentResults;
using MainCore.DTO;

namespace MainCore.Features.Proxy.Commands
{
    public interface IValidateCommand
    {
        Task<Result> Execute(AccessDto access);
    }
}