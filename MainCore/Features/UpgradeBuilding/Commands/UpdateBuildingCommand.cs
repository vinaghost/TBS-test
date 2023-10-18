using FluentResults;
using MainCore.Common.Errors;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    [RegisterAsTransient]
    public class UpdateBuildingCommand : IUpdateBuildingCommand
    {
        private readonly IUpdateFieldCommand _updateFieldCommand;
        private readonly IUpdateInfrastructureCommand _updateInfrastructureCommand;
        private readonly IUpdateQueueBuildingCommand _updateQueueBuildingCommand;
        private readonly IUpdateAccountInfoCommand _updateAccountInfoCommand;
        private readonly IChromeManager _chromeManager;

        public UpdateBuildingCommand(IUpdateFieldCommand updateFieldCommand, IUpdateInfrastructureCommand updateInfrastructureCommand, IUpdateQueueBuildingCommand updateQueueBuildingCommand, IChromeManager chromeManager, IUpdateAccountInfoCommand updateAccountInfoCommand)
        {
            _updateFieldCommand = updateFieldCommand;
            _updateInfrastructureCommand = updateInfrastructureCommand;
            _updateQueueBuildingCommand = updateQueueBuildingCommand;
            _chromeManager = chromeManager;
            _updateAccountInfoCommand = updateAccountInfoCommand;
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var currentUrl = chromeBrowser.CurrentUrl;
            Result result;

            result = await _updateAccountInfoCommand.Execute(accountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            if (currentUrl.Contains("dorf"))
            {
                result = await _updateQueueBuildingCommand.Execute(chromeBrowser, villageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                if (currentUrl.Contains("dorf1"))
                {
                    result = await _updateFieldCommand.Execute(chromeBrowser, villageId);
                    if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                }
                else if (currentUrl.Contains("dorf2"))
                {
                    result = await _updateInfrastructureCommand.Execute(chromeBrowser, villageId);
                    if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                }
            }

            return Result.Ok();
        }
    }
}