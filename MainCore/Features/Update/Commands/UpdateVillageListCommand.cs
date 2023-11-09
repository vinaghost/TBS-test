using FluentResults;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MainCore.Notification;
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
            var villages = context.Villages
                .Where(x => x.AccountId == accountId.Value)
                .ToList();

            var ids = dtos.Select(x => x.Id.Value).ToList();

            var villageDeleted = villages.Where(x => !ids.Contains(x.Id)).ToList();
            var villageInserted = dtos.Where(x => !villages.Any(v => v.Id == x.Id.Value)).ToList();
            var villageUpdated = villages.Where(x => ids.Contains(x.Id)).ToList();

            villageDeleted.ForEach(x => context.Remove(x));
            villageInserted.ForEach(x => context.Add(x.ToEntity(accountId)));

            foreach (var village in villageUpdated)
            {
                var dto = dtos.FirstOrDefault(x => x.Id.Value == village.Id);
                dto.To(village);
                context.Update(village);
            }

            context.SaveChanges();
        }
    }
}