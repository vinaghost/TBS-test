using FluentResults;
using MainCore.Common.Errors;
using MainCore.Features.Navigate.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateDorfCommand : IUpdateDorfCommand
    {
        private readonly IUpdateFieldCommand _updateFieldCommand;
        private readonly IUpdateInfrastructureCommand _updateInfrastructureCommand;
        private readonly IChromeManager _chromeManager;
        private readonly IUpdateStorageCommand _updateStorageCommand;
        private readonly IUpdateQueueBuildingCommand _updateQueueBuildingCommand;
        private readonly IToDorfCommand _toDorfCommand;

        public UpdateDorfCommand(IUpdateFieldCommand updateFieldCommand, IUpdateInfrastructureCommand updateInfrastructureCommand, IChromeManager chromeManager, IUpdateStorageCommand updateStorageCommand, IUpdateQueueBuildingCommand updateQueueBuildingCommand, IToDorfCommand toDorfCommand)
        {
            _updateFieldCommand = updateFieldCommand;
            _updateInfrastructureCommand = updateInfrastructureCommand;
            _chromeManager = chromeManager;
            _updateStorageCommand = updateStorageCommand;
            _updateQueueBuildingCommand = updateQueueBuildingCommand;
            _toDorfCommand = toDorfCommand;
        }

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId)
        {
            Result result;
            var url = chromeBrowser.CurrentUrl;
            if (url.Contains("dorf1"))
            {
                result = await _updateQueueBuildingCommand.Execute(chromeBrowser, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _updateFieldCommand.Execute(chromeBrowser, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            else if (url.Contains("dorf2"))
            {
                result = await _updateQueueBuildingCommand.Execute(chromeBrowser, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _updateInfrastructureCommand.Execute(chromeBrowser, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            else
            {
                result = await _toDorfCommand.Execute(chromeBrowser, 1);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _updateFieldCommand.Execute(chromeBrowser, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            result = await _updateStorageCommand.Execute(chromeBrowser, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, villageId);
        }
    }
}