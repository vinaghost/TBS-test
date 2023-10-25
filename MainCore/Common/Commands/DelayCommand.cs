using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MediatR;

namespace MainCore.Common.Commands
{
    public class DelayCommand : IRequest<Result>
    {
        public AccountId AccountId { get; }

        public DelayCommand(AccountId accountId)
        {
            AccountId = accountId;
        }
    }

    public class DelayCommandHandler : IRequestHandler<DelayCommand, Result>
    {
        private readonly IAccountSettingRepository _settingRepository;

        public DelayCommandHandler(IAccountSettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<Result> Handle(DelayCommand request, CancellationToken cancellationToken)
        {
            var delay = _settingRepository.GetSetting(request.AccountId, AccountSettingEnums.ClickDelayMin, AccountSettingEnums.ClickDelayMax);
            await Task.Delay(delay, cancellationToken);
            return Result.Ok();
        }
    }
}