using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Errors.Farming;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MediatR;

namespace MainCore.Features.Farming.Commands
{
    [RegisterAsTransient]
    public class ToFarmListPageCommand : IToFarmListPageCommand
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IVillageRepository _villageRepository;

        private readonly ISwitchVillageCommand _switchVillageCommand;
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IToBuildingCommand _toBuildingCommand;
        private readonly ISwitchTabCommand _switchTabCommand;

        private readonly IMediator _mediator;

        public ToFarmListPageCommand(IBuildingRepository buildingRepository, IVillageRepository villageRepository, ISwitchVillageCommand switchVillageCommand, IToDorfCommand toDorfCommand, IToBuildingCommand toBuildingCommand, ISwitchTabCommand switchTabCommand, IMediator mediator)
        {
            _buildingRepository = buildingRepository;
            _villageRepository = villageRepository;
            _switchVillageCommand = switchVillageCommand;
            _toDorfCommand = toDorfCommand;
            _toBuildingCommand = toBuildingCommand;
            _switchTabCommand = switchTabCommand;
            _mediator = mediator;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            Result result;
            result = await _mediator.Send(new UpdateVillageListCommand(accountId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var rallypointVillageId = await GetVillageHasRallypoint(accountId);
            if (rallypointVillageId == VillageId.Empty) return Result.Fail(new NoRallypoint());

            result = await _switchVillageCommand.Execute(accountId, rallypointVillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _toDorfCommand.Execute(accountId, 2);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _toBuildingCommand.Execute(accountId, 39);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _switchTabCommand.Execute(accountId, 4);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        private async Task<VillageId> GetVillageHasRallypoint(AccountId accountId)
        {
            var activeVillage = await _villageRepository.GetActiveVillageId(accountId);
            if (_buildingRepository.HasRallyPoint(activeVillage))
            {
                return activeVillage;
            }

            var inactiveVillages = await _villageRepository.GetInactiveVillageId(accountId);
            foreach (var village in inactiveVillages)
            {
                if (_buildingRepository.HasRallyPoint(village))
                {
                    return village;
                }
            }
            return VillageId.Empty;
        }
    }
}