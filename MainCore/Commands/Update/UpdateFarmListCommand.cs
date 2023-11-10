using FluentResults;
using MainCore.Common;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Notification;
using MediatR;

namespace MainCore.Commands.Update
{
    [RegisterAsTransient]
    public class UpdateFarmListCommand : IUpdateFarmListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFarmListCommand(IChromeManager chromeManager, IMediator mediator, IUnitOfWork unitOfWork)
        {
            _chromeManager = chromeManager;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _unitOfWork.FarmParser.Get(html);
            await Task.Run(() => _unitOfWork.FarmRepository.Update(accountId, dtos.ToList()));
            await _mediator.Publish(new FarmListUpdated(accountId));
            return Result.Ok();
        }
    }
}