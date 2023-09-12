using FluentResults;
using MainCore.Errors;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
using NavigateCore.Commands;
using UpdateCore.Commands;

namespace UpdateCore.Tasks
{
    public class UpdateVillageTask : VillageTask
    {
        private readonly IVillageRepository _villageRepository;
        private readonly ISwitchVillageCommand _switchVillageCommand;
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IUpdateFieldCommand _updateFieldCommand;
        private readonly IUpdateInfrastructureCommand _updateInfrastructureCommand;
        private readonly IChromeManager _chromeManager;

        public UpdateVillageTask(IVillageRepository villageRepository, ISwitchVillageCommand switchVillageCommand, IToDorfCommand toDorfCommand, IUpdateFieldCommand updateFieldCommand, IUpdateInfrastructureCommand updateInfrastructureCommand, IChromeManager chromeManager)
        {
            _villageRepository = villageRepository;
            _switchVillageCommand = switchVillageCommand;
            _toDorfCommand = toDorfCommand;
            _updateFieldCommand = updateFieldCommand;
            _updateInfrastructureCommand = updateInfrastructureCommand;
            _chromeManager = chromeManager;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = await _switchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var chromeBrowser = _chromeManager.Get(AccountId);
            var url = chromeBrowser.CurrentUrl;
            if (url.Contains("dorf1"))
            {
                result = await _updateFieldCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _toDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _updateInfrastructureCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            else if (url.Contains("dorf2"))
            {
                result = await _updateInfrastructureCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _toDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _updateFieldCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            else
            {
                result = await _toDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _updateInfrastructureCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _toDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _updateFieldCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        protected override async Task SetName()
        {
            var village = await _villageRepository.Get(VillageId);
            _name = $"Update buildings in {village.Name}";
        }
    }
}