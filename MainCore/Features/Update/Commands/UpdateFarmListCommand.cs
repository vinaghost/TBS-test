using FluentResults;
using MainCore.Notification;
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
            var dbFarmLists = context.FarmLists
                .Where(x => x.AccountId == accountId.Value)
                .ToList();

            foreach (var dto in dtos)
            {
                var dbFarmlist = dbFarmLists
                    .Where(x => x.Id == dto.Id.Value)
                    .FirstOrDefault();
                if (dbFarmlist is null)
                {
                    var farmlist = dto.ToEntity(accountId);
                    context.Add(farmlist);
                }
                else
                {
                    dto.To(dbFarmlist);
                    context.Update(dbFarmlist);
                    dbFarmLists.Remove(dbFarmlist);
                }
            }
            context.SaveChanges();

            var removedFarmLists = dbFarmLists.Select(x => x.Id).AsEnumerable();
            context.FarmLists
                .Where(x => removedFarmLists.Contains(x.Id))
                .ExecuteDelete();
        }
    }
}