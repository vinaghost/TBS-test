using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Features.InstantUpgrade.Commands;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.Update.Commands;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Features.InstantUpgrade.Tasks
{
    public class InstantUpgradeTask : VillageTask
    {
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IInstantUpgradeCommand _instantUpgradeCommand;
        private readonly ISwitchVillageCommand _switchVillageCommand;
        private readonly ITaskManager _taskManager;
        private readonly IMediator _mediator;

        public InstantUpgradeTask(IToDorfCommand toDorfCommand, IInstantUpgradeCommand instantUpgradeCommand, ISwitchVillageCommand switchVillageCommand, ITaskManager taskManager, IMediator mediator)
        {
            _toDorfCommand = toDorfCommand;
            _instantUpgradeCommand = instantUpgradeCommand;
            _switchVillageCommand = switchVillageCommand;
            _taskManager = taskManager;
            _mediator = mediator;
        }

        public override async Task<Result> Execute()
        {
            Result result;
            result = _switchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _toDorfCommand.Execute(AccountId, 1);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _instantUpgradeCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _mediator.Send(new UpdateDorfCommand(AccountId, VillageId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            TriggerUpgradeBuilding();
            return Result.Ok();
        }

        protected override void SetName()
        {
            _name = "InstantUpgradeTask";
        }

        private void TriggerUpgradeBuilding()
        {
            var task = _taskManager.Get<UpgradeBuildingTask>(AccountId, VillageId);
            if (task is null) return;
            task.ExecuteAt = DateTime.Now;
            _taskManager.ReOrder(AccountId);
        }
    }
}