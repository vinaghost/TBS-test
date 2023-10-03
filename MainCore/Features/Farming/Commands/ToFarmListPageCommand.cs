using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Errors.Farming;
using MainCore.Common.Repositories;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Farming.Commands
{
    [RegisterAsTransient]
    public class ToFarmListPageCommand : IToFarmListPageCommand
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IVillageRepository _villageRepository;

        private readonly IUpdateVillageListCommand _updateVillageListCommand;
        private readonly ISwitchVillageCommand _switchVillageCommand;
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IToBuildingCommand _toBuildingCommand;
        private readonly ISwitchTabCommand _switchTabCommand;

        public ToFarmListPageCommand(IBuildingRepository buildingRepository, IVillageRepository villageRepository, IUpdateVillageListCommand updateVillageListCommand, ISwitchVillageCommand switchVillageCommand, IToDorfCommand toDorfCommand, IToBuildingCommand toBuildingCommand, ISwitchTabCommand switchTabCommand)
        {
            _buildingRepository = buildingRepository;
            _villageRepository = villageRepository;
            _updateVillageListCommand = updateVillageListCommand;
            _switchVillageCommand = switchVillageCommand;
            _toDorfCommand = toDorfCommand;
            _toBuildingCommand = toBuildingCommand;
            _switchTabCommand = switchTabCommand;
        }

        public async Task<Result> Execute(int accountId)
        {
            Result result;
            result = await _updateVillageListCommand.Execute(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var rallypointVillageId = await GetVillageHasRallypoint(accountId);
            if (rallypointVillageId == 0) return Result.Fail(new NoRallypoint());

            result = await _switchVillageCommand.Execute(accountId, rallypointVillageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _toDorfCommand.Execute(accountId, 2);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _toBuildingCommand.Execute(accountId, 39);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _switchTabCommand.Execute(accountId, 4);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private async Task<int> GetVillageHasRallypoint(int accountId)
        {
            var activeVillage = await _villageRepository.GetActive(accountId);
            if (await _buildingRepository.HasRallyPoint(activeVillage.Id))
            {
                return activeVillage.Id;
            }

            var inactiveVillages = await _villageRepository.GetInactive(accountId);
            foreach (var village in inactiveVillages)
            {
                if (await _buildingRepository.HasRallyPoint(village.Id))
                {
                    return activeVillage.Id;
                }
            }
            return 0;
        }
    }
}