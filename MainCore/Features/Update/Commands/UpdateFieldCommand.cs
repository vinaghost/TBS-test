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
    public class UpdateFieldCommand : IUpdateFieldCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IFieldParser _fieldParser;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IQueueBuildingRepository _queueBuildingRepository;
        private readonly IMediator _mediator;

        public UpdateFieldCommand(IChromeManager chromeManager, IFieldParser fieldParser, IBuildingRepository buildingRepository, IQueueBuildingRepository queueBuildingRepository, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _fieldParser = fieldParser;
            _buildingRepository = buildingRepository;
            _queueBuildingRepository = queueBuildingRepository;
            _mediator = mediator;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.Html;

            var dtos = _fieldParser.Get(html);

            var mapper = new BuildingMapper();
            var buildings = new List<Building>();
            foreach (var dto in dtos)
            {
                var field = mapper.Map(villageId, dto);
                buildings.Add(field);
            }

            _buildingRepository.Update(villageId, buildings);
            var isUnderConstructionList = buildings.Where(x => x.IsUnderConstruction).ToList();
            if (isUnderConstructionList.Count > 0)
            {
                _queueBuildingRepository.Update(villageId, isUnderConstructionList);
                await _mediator.Send(new QueueBuildingUpdate(villageId));
            }
            else
            {
                await _mediator.Send(new BuildingUpdate(villageId));
            }
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, villageId);
        }
    }
}