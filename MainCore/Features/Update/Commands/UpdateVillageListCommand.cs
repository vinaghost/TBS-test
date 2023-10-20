using FluentResults;
using MainCore.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Features.Update.Trigger;
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
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        public UpdateVillageListCommandCommandHandler(IChromeManager chromeManager, AppDbContext context, IVillageListParser villageListParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _context = context;
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
            await _mediator.Publish(new VillageTrigger(accountId), cancellationToken);
            return Result.Ok();
        }

        private void Update(int accountId, List<VillageDto> dtos)
        {
           
            var query = _context.Villages.Where(x => x.AccountId == accountId);
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
                    _context.Add(VillageList);
                }
                else
                {
                    mapper.MapToEntity(dto, dbVillage);
                    _context.Update(dbVillage);
                }

                ids.Remove(dto.Id);
            }
            _context.SaveChanges();

            _context.Villages
                .Where(x => ids.Contains(x.Id))
                .ExecuteDelete();
        }
    }
}