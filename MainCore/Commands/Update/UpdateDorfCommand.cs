using FluentResults;
using HtmlAgilityPack;
using MainCore.Common;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Notification;
using MediatR;

namespace MainCore.Commands.Update
{
    [RegisterAsTransient]
    public class UpdateDorfCommand : IUpdateDorfCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDorfCommand(IChromeManager chromeManager, IMediator mediator, IUnitOfWork unitOfWork)
        {
            _chromeManager = chromeManager;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Execute(AccountId accountId, VillageId villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var dtoQueueBuilding = _unitOfWork.QueueBuildingParser.Get(html);
            var dtoBuilding = GetBuildings(chromeBrowser.CurrentUrl, html);
            var dtoStorage = _unitOfWork.StockBarParser.Get(html);

            var queueBuildings = dtoQueueBuilding.ToList();
            var result = IsVaild(queueBuildings);
            if (result.IsFailed) return result;

            await Task.Run(() => _unitOfWork.QueueBuildingRepository.Update(villageId, queueBuildings));

            await Task.Run(() => _unitOfWork.BuildingRepository.Update(villageId, dtoBuilding.ToList()));

            var dtoUnderConstructionBuildings = dtoBuilding.Where(x => x.IsUnderConstruction).ToList();
            await Task.Run(() => _unitOfWork.QueueBuildingRepository.Update(villageId, dtoUnderConstructionBuildings));
            await _mediator.Publish(new QueueBuildingUpdated(villageId));

            await Task.Run(() => _unitOfWork.StorageRepository.Update(villageId, dtoStorage));

            return Result.Ok();
        }

        private IEnumerable<BuildingDto> GetBuildings(string url, HtmlDocument html)
        {
            if (url.Contains("dorf1"))
                return _unitOfWork.FieldParser.Get(html);

            if (url.Contains("dorf2"))
                return _unitOfWork.InfrastructureParser.Get(html);

            return Enumerable.Empty<BuildingDto>();
        }

        private static Result IsVaild(List<QueueBuildingDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var strType = dto.Type;
                var resultParse = Enum.TryParse(strType, false, out BuildingEnums _);
                if (!resultParse) return Result.Fail(Stop.EnglishRequired(strType));
            }
            return Result.Ok();
        }
    }
}