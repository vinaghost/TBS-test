using FluentResults;
using MainCore.Commands.Base;
using MainCore.Entities;
using MediatR;

namespace MainCore.Commands.Special
{
    public class LoginCommand : ByAccountIdRequestBase, IRequest<Result>
    {
        public LoginCommand(AccountId accountId) : base(accountId)
        {
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result>
    {
        private readonly IUnitOfCommand _unitOfCommand;

        public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await Task.Run(() => _unitOfCommand.InputLoginCommand.Execute(request.AccountId), cancellationToken);
            return result;
        }
    }
}