using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Repositories;
using MainCore.Common.Tasks;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class UpdateVillageTask : VillageTask
    {
        private readonly IVillageRepository _villageRepository;
        private readonly ISwitchVillageCommand _switchVillageCommand;
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IUpdateFieldCommand _updateFieldCommand;
        private readonly IUpdateInfrastructureCommand _updateInfrastructureCommand;
        private readonly IChromeManager _chromeManager;
        private readonly IUpdateStorageCommand _updateStorageCommand;
        private readonly IUpdateQueueBuildingCommand _updateQueueBuildingCommand;

        public UpdateVillageTask(IVillageRepository villageRepository, ISwitchVillageCommand switchVillageCommand, IToDorfCommand toDorfCommand, IUpdateFieldCommand updateFieldCommand, IUpdateInfrastructureCommand updateInfrastructureCommand, IChromeManager chromeManager, IUpdateStorageCommand updateStorageCommand, IUpdateQueueBuildingCommand updateQueueBuildingCommand)
        {
            _villageRepository = villageRepository;
            _switchVillageCommand = switchVillageCommand;
            _toDorfCommand = toDorfCommand;
            _updateFieldCommand = updateFieldCommand;
            _updateInfrastructureCommand = updateInfrastructureCommand;
            _chromeManager = chromeManager;
            _updateStorageCommand = updateStorageCommand;
            _updateQueueBuildingCommand = updateQueueBuildingCommand;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = await _switchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var chromeBrowser = _chromeManager.Get(AccountId);
            var url = chromeBrowser.CurrentUrl;
            if (url.Contains("dorf1"))
            {
                result = await _updateQueueBuildingCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _updateFieldCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _toDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _updateInfrastructureCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else if (url.Contains("dorf2"))
            {
                result = await _updateQueueBuildingCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _updateInfrastructureCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _toDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _updateFieldCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else
            {
                result = await _toDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _updateInfrastructureCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _toDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _updateFieldCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }

            result = await _updateStorageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }

        protected override void SetName()
        {
            var village = _villageRepository.Get(VillageId);
            _name = $"Update buildings in {village.Name}";
        }
    }
}