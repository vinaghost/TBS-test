using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Common.Requests;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateFarmListCommand : IUpdateFarmListCommand
    {
        private readonly IFarmListRepository _farmListRepository;
        private readonly IChromeManager _chromeManager;
        private readonly IFarmListParser _farmListParser;
        private readonly IMediator _mediator;

        public UpdateFarmListCommand(IFarmListRepository farmListRepository, IChromeManager chromeManager, IFarmListParser farmListParser, IMediator mediator)
        {
            _farmListRepository = farmListRepository;
            _chromeManager = chromeManager;
            _farmListParser = farmListParser;
            _mediator = mediator;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var dtos = _farmListParser.Get(html);
            var mapper = new FarmListMapper();

            var farmLists = new List<FarmList>();

            foreach (var dto in dtos)
            {
                var farmList = mapper.Map(accountId, dto);
                farmLists.Add(farmList);
            }

            _farmListRepository.Update(accountId, farmLists);
            await _mediator.Send(new FarmListUpdate(accountId));
            return Result.Ok();
        }
    }
}