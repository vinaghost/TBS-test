﻿using FluentResults;
using MainCore.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Features.Update.Commands
{
    public class UpdateVillageListCommand : IRequest<Result>
    {
        public int AccountId { get; }

        public UpdateVillageListCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateVillageListCommandCommandHandler : IRequestHandler<UpdateVillageListCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IVillageListParser _villageListParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UpdateVillageListCommandCommandHandler(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IVillageListParser villageListParser)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _villageListParser = villageListParser;
        }

        public async Task<Result> Handle(UpdateVillageListCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _villageListParser.Get(html);
            await Task.Run(() => Update(accountId, dtos.ToList()), cancellationToken);
            return Result.Ok();
        }

        private void Update(int accountId, List<VillageDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Villages.Where(x => x.AccountId == accountId);
            var ids = query
                .Select(x => x.Id)
                .ToList();

            var dbVillages = query.ToList();

            var mapper = new VillageMapper();
            foreach (var dto in dtos)
            {
                var dbVillage = dbVillages.FirstOrDefault(x => x.Id == dto.Id);
                if (dbVillage is null)
                {
                    var VillageList = mapper.Map(accountId, dto);
                    context.Add(VillageList);
                }
                else
                {
                    mapper.MapToEntity(dto, dbVillage);
                    context.Update(dbVillage);
                }

                ids.Remove(dto.Id);
            }
            context.SaveChanges();

            context.Villages
                .Where(x => ids.Contains(x.Id))
                .ExecuteDelete();
        }
    }
}