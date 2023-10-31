using FluentResults;
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
    public class UpdateFarmListCommand : IRequest<Result>
    {
        public AccountId AccountId { get; }

        public UpdateFarmListCommand(AccountId accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateFarmListCommandCommandHandler : IRequestHandler<UpdateFarmListCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IFarmListParser _farmListParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public UpdateFarmListCommandCommandHandler(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IFarmListParser farmListParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _farmListParser = farmListParser;
            _mediator = mediator;
        }

        public async Task<Result> Handle(UpdateFarmListCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _farmListParser.Get(html);
            await Task.Run(() => Update(accountId, dtos.ToList()), cancellationToken);
            await _mediator.Publish(new FarmListUpdated(accountId), cancellationToken);
            return Result.Ok();
        }

        private void Update(AccountId accountId, List<FarmListDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.FarmLists
                .Where(x => x.AccountId == accountId);

            var ids = query
                .Select(x => x.Id)
                .ToList();

            var dbFarmLists = query.ToList();

            var mapper = new FarmListMapper();
            foreach (var dto in dtos)
            {
                var dbFarmlist = dbFarmLists.FirstOrDefault(x => x.Id == dto.Id);
                if (dbFarmlist is null)
                {
                    var farmlist = mapper.Map(accountId, dto);
                    context.Add(farmlist);
                }
                else
                {
                    mapper.MapToEntity(dto, dbFarmlist);
                    context.Update(dbFarmlist);
                }

                ids.Remove(dto.Id);
            }
            context.SaveChanges();

            context.FarmLists
                .Where(x => ids.Contains(x.Id))
                .ExecuteDelete();
        }
    }
}