using FluentResults;
using MainCore.DTO;

namespace MainCore.Commands.Validate
{
    public interface IValidateProxyCommand
    {
        Task<Result> Execute(AccessDto access);
    }
}