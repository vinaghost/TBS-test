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
    public class UpdateVillageListCommand : IRequest<Result>
    {
        public AccountId AccountId { get; }

        public UpdateVillageListCommand(AccountId accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateVillageListCommandCommandHandler : IRequestHandler<UpdateVillageListCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IVillageListParser _villageListParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public UpdateVillageListCommandCommandHandler(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IVillageListParser villageListParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _villageListParser = villageListParser;
            _mediator = mediator;
        }

        public async Task<Result> Handle(UpdateVillageListCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _villageListParser.Get(html);
            await Task.Run(() => Update(accountId, dtos.ToList()), cancellationToken);
            await _mediator.Publish(new VillageUpdated(accountId), cancellationToken);
            return Result.Ok();
        }

        private void Update(AccountId accountId, List<VillageDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbVillages = context.Villages
                .Where(x => x.AccountId == accountId.Value)
                .ToList();

            foreach (var dto in dtos)
            {
                var dbVillage = dbVillages.FirstOrDefault(x => x.Id == dto.Id.Value);
                if (dbVillage is null)
                {
                    var entity = dto.ToEntity(accountId);
                    context.Add(entity);
                }
                else
                {
                    dto.To(dbVillage);
                    context.Update(dbVillage);
                    dbVillages.Remove(dbVillage);
                }
            }
            context.SaveChanges();

            var removedVillage = dbVillages.Select(x => x.Id).AsEnumerable();
            context.Villages
                .Where(x => removedVillage.Contains(x.Id))
                .ExecuteDelete();
        }
    }
}