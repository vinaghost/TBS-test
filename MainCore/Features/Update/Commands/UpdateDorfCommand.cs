﻿using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Features.Update.Trigger;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Features.Update.Commands
{
    public class UpdateDorfCommand : IRequest<Result>
    {
        public int VillageId { get; }
        public int AccountId { get; }

        public UpdateDorfCommand(int accountId, int villageId)
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
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        public UpdateDorfCommandHandler(IChromeManager chromeManager, AppDbContext context, IQueueBuildingParser queueBuildingParser, IFieldParser fieldParser, IInfrastructureParser infrastructureParser, IStockBarParser stockBarParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _context = context;
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

            var result = await Task.Run(() => UpdateQueueBuilding(accountId, dtoQueueBuilding), cancellationToken);
            if (result.IsFailed) return result;

            await Task.Run(() => UpdateBuildings(villageId, dtoBuilding), cancellationToken);

            var dtoUnderConstructionBuildings = dtoBuilding.Where(x => x.IsUnderConstruction).ToList();
            await Task.WhenAll(
                new[]
                {
                    Task.Run(() => UpdateQueueBuilding(villageId, dtoUnderConstructionBuildings), cancellationToken),
                    Task.Run(() => UpdateStorage(villageId, dtoStorage), cancellationToken),
                }
            );
            await _mediator.Publish(new QueueBuildingTrigger(villageId), cancellationToken);
            await _mediator.Publish(new BuildingTrigger(villageId), cancellationToken);
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
                var resultParse = Enum.TryParse(strType, false, out BuildingEnums type);
                if (!resultParse) return Result.Fail(Stop.EnglishRequired(strType));
            }
            return Result.Ok();
        }

        private Result UpdateQueueBuilding(int villageId, IEnumerable<QueueBuildingDto> dtos)
        {
            var result = IsVaild(dtos);
            if (result.IsFailed) return result;

           
            var mapper = new QueueBuildingMapper();

            _context.QueueBuildings
                .Where(x => x.VillageId == villageId)
                .ExecuteDelete();

            foreach (var dto in dtos)
            {
                var queueBuilding = mapper.Map(villageId, dto);
                _context.Add(queueBuilding);
            }
            _context.SaveChanges();
            return Result.Ok();
        }

        private void UpdateQueueBuilding(int villageId, List<BuildingDto> dtos)
        {
           

            var queueBuildings = _context.QueueBuildings
                .Where(x => x.VillageId == villageId && x.Type != BuildingEnums.Site);

            if (dtos.Count == 1)
            {
                var building = dtos[0];
                queueBuildings = queueBuildings.Where(x => x.Type == building.Type);
                var list = queueBuildings.ToList();
                foreach (var item in list)
                {
                    item.Location = building.Location;
                }
                _context.UpdateRange(list);
            }
            else if (dtos.Count == 2)
            {
                foreach (var dto in dtos)
                {
                    var queueBuilding = queueBuildings.FirstOrDefault(x => x.Type == dto.Type);
                    queueBuilding.Location = dto.Location;
                    _context.Update(queueBuilding);
                }
            }
            _context.SaveChanges();
        }

        private void UpdateBuildings(int villageId, IEnumerable<BuildingDto> dtos)
        {
           

            var dbBuildings = _context.Buildings
                .Where(x => x.VillageId == villageId)
                .ToList();

            var mapper = new BuildingMapper();
            foreach (var dto in dtos)
            {
                var dbBuilding = dbBuildings
                    .FirstOrDefault(x => x.Location == dto.Location);
                if (dbBuilding is null)
                {
                    var building = mapper.Map(villageId, dto);
                    _context.Add(building);
                }
                else
                {
                    mapper.MapToEntity(dto, dbBuilding);
                    _context.Update(dbBuilding);
                }
            }
            _context.SaveChanges();
        }

        private void UpdateStorage(int villageId, StorageDto dto)
        {
           

            var mapper = new StorageMapper();
            var dbStorage = _context.Storages.FirstOrDefault(x => x.VillageId == villageId);
            if (dbStorage is null)
            {
                var storage = mapper.Map(villageId, dto);
                _context.Add(storage);
            }
            else
            {
                mapper.MapToEntity(dto, dbStorage);
                _context.Update(dbStorage);
            }

            _context.SaveChanges();
        }
    }
}