using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Notification;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Features.Update.Commands
{
    public class UpdateDorfCommand : IRequest<Result>
    {
        public VillageId VillageId { get; }
        public AccountId AccountId { get; }

        public UpdateDorfCommand(AccountId accountId, VillageId villageId)
        {
            VillageId = villageId;
            AccountId = accountId;
        }
    }

    public class UpdateDorfCommandHandler : IRequestHandler<UpdateDorfCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IQueueBuildingParser _queueBuildingParser;
        private readonly IFieldParser _fieldParser;
        private readonly IInfrastructureParser _infrastructureParser;
        private readonly IStockBarParser _stockBarParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public UpdateDorfCommandHandler(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IQueueBuildingParser queueBuildingParser, IFieldParser fieldParser, IInfrastructureParser infrastructureParser, IStockBarParser stockBarParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _queueBuildingParser = queueBuildingParser;
            _fieldParser = fieldParser;
            _infrastructureParser = infrastructureParser;
            _stockBarParser = stockBarParser;
            _mediator = mediator;
        }

        public async Task<Result> Handle(UpdateDorfCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var villageId = request.VillageId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var dtoQueueBuilding = _queueBuildingParser.Get(html);
            var dtoBuilding = GetBuildings(chromeBrowser.CurrentUrl, html);
            var dtoStorage = _stockBarParser.Get(html);

            var result = await Task.Run(() => UpdateQueueBuilding(villageId, dtoQueueBuilding), cancellationToken);
            if (result.IsFailed) return result;

            await Task.Run(() => UpdateBuildings(villageId, dtoBuilding), cancellationToken);

            var dtoUnderConstructionBuildings = dtoBuilding.Where(x => x.IsUnderConstruction).ToList();
            await Task.Run(() => UpdateQueueBuilding(villageId, dtoUnderConstructionBuildings), cancellationToken);
            await _mediator.Publish(new QueueBuildingUpdated(villageId), cancellationToken);

            await Task.Run(() => UpdateStorage(villageId, dtoStorage), cancellationToken);

            return Result.Ok();
        }

        private IEnumerable<BuildingDto> GetBuildings(string url, HtmlDocument html)
        {
            if (url.Contains("dorf1"))
                return _fieldParser.Get(html);

            if (url.Contains("dorf2"))
                return _infrastructureParser.Get(html);

            return Enumerable.Empty<BuildingDto>();
        }

        private static Result IsVaild(IEnumerable<QueueBuildingDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var strType = dto.Type;
                var resultParse = Enum.TryParse(strType, false, out BuildingEnums _);
                if (!resultParse) return Result.Fail(Stop.EnglishRequired(strType));
            }
            return Result.Ok();
        }

        private Result UpdateQueueBuilding(VillageId villageId, IEnumerable<QueueBuildingDto> dtos)
        {
            var result = IsVaild(dtos);
            if (result.IsFailed) return result;

            using var context = _contextFactory.CreateDbContext();

            context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .ExecuteDelete();

            var entities = new List<QueueBuilding>();

            foreach (var dto in dtos)
            {
                var queueBuilding = dto.ToEntity(villageId);
                entities.Add(queueBuilding);
            }
            context.AddRange(entities);
            context.SaveChanges();
            return Result.Ok();
        }

        private void UpdateQueueBuilding(VillageId villageId, List<BuildingDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();
            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type != BuildingEnums.Site);

            if (dtos.Count == 1)
            {
                var building = dtos[0];
                queueBuildings = queueBuildings
                    .Where(x => x.Type == building.Type);

                var list = queueBuildings.ToList();
                foreach (var item in list)
                {
                    item.Location = building.Location;
                }
                context.UpdateRange(list);
            }
            else if (dtos.Count == 2)
            {
                foreach (var dto in dtos)
                {
                    var list = queueBuildings.ToList();
                    var queueBuilding = list.FirstOrDefault(x => x.Type == dto.Type);
                    queueBuilding.Location = dto.Location;
                    context.Update(queueBuilding);
                }
            }
            context.SaveChanges();
        }

        private void UpdateBuildings(VillageId villageId, IEnumerable<BuildingDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbBuildings = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .ToList();

            foreach (var dto in dtos)
            {
                var dbBuilding = dbBuildings
                    .FirstOrDefault(x => x.Location == dto.Location);
                if (dbBuilding is null)
                {
                    var building = dto.ToEntity(villageId);
                    context.Add(building);
                }
                else
                {
                    dto.To(dbBuilding);
                    context.Update(dbBuilding);
                }
            }
            context.SaveChanges();
        }

        private void UpdateStorage(VillageId villageId, StorageDto dto)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbStorage = context.Storages
                .Where(x => x.VillageId == villageId.Value)
                .FirstOrDefault();

            if (dbStorage is null)
            {
                var storage = dto.ToEntity(villageId);
                context.Add(storage);
            }
            else
            {
                dto.To(dbStorage);
                context.Update(dbStorage);
            }

            context.SaveChanges();
        }
    }
}