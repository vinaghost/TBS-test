﻿using FluentResults;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Features.Update.Commands
{
    public class UpdateHeroItemsCommand : IRequest<Result>
    {
        public AccountId AccountId { get; }

        public UpdateHeroItemsCommand(AccountId accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateHeroItemsCommandCommandHandler : IRequestHandler<UpdateHeroItemsCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IHeroParser _heroParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UpdateHeroItemsCommandCommandHandler(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IHeroParser heroParser)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _heroParser = heroParser;
        }

        public async Task<Result> Handle(UpdateHeroItemsCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _heroParser.GetItems(html);
            await Task.Run(() => Update(accountId, dtos.ToList()), cancellationToken);
            return Result.Ok();
        }

        private void Update(AccountId accountId, List<HeroItemDto> dtos)
        {
            var query = _context.HeroItems.Where(x => x.AccountId == accountId);
            var ids = query
                .Select(x => x.Type)
                .ToList();

            var dbHeroItemss = query.ToList();

            var mapper = new HeroItemMapper();
            foreach (var dto in dtos)
            {
                var dbHeroItems = dbHeroItemss.FirstOrDefault(x => x.Type == dto.Type);
                if (dbHeroItems is null)
                {
                    var HeroItems = mapper.Map(accountId, dto);
                    _context.Add(HeroItems);
                }
                else
                {
                    mapper.MapToEntity(dto, dbHeroItems);
                    _context.Update(dbHeroItems);
                }

                ids.Remove(dto.Type);
            }
            _context.SaveChanges();

            _context.HeroItems
                .Where(x => x.AccountId == accountId)
                .Where(x => ids.Contains(x.Type))
                .ExecuteDelete();
        }
    }
}