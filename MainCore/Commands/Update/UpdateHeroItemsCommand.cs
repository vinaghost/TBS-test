using FluentResults;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Notification.Message;
using MainCore.Parsers;
using MainCore.Repositories;
using MainCore.Services;
using MediatR;

namespace MainCore.Commands.Update
{
    [RegisterAsTransient]
    public class UpdateHeroItemsCommand : IUpdateHeroItemsCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfParser _unitOfParser;

        public UpdateHeroItemsCommand(IChromeManager chromeManager, IMediator mediator, IUnitOfRepository unitOfRepository, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _mediator = mediator;
            _unitOfRepository = unitOfRepository;
            _unitOfParser = unitOfParser;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _unitOfParser.HeroParser.Get(html);
            await Task.Run(() => _unitOfRepository.HeroItemRepository.Update(accountId, dtos.ToList()));
            await _mediator.Publish(new HeroItemUpdated(accountId));
            return Result.Ok();
        }
    }
}