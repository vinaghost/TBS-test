using MainCore.Common.Enums;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class SaveAccountSettingByIdCommand : ByAccountIdRequestBase, IRequest
    {
        public Dictionary<AccountSettingEnums, int> Settings { get; }

        public SaveAccountSettingByIdCommand(AccountId accountId, Dictionary<AccountSettingEnums, int> settings) : base(accountId)
        {
            Settings = settings;
        }
    }

    public class SaveAccountSettingByIdCommandHandler : IRequestHandler<SaveAccountSettingByIdCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveAccountSettingByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SaveAccountSettingByIdCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.AccountSettingRepository.Update(request.AccountId, request.Settings), cancellationToken);
        }
    }
}