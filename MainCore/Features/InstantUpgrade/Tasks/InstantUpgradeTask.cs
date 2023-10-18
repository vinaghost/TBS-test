using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Features.InstantUpgrade.Commands;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.Update.Commands;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.InstantUpgrade.Tasks
{
    public class InstantUpgradeTask : VillageTask
    {
        private readonly IToDorfCommand _toDorfCommand;
        private readonly IInstantUpgradeCommand _instantUpgradeCommand;
        private readonly IUpdateDorfCommand _updateDorfCommand;
        private readonly ISwitchVillageCommand _switchVillageCommand;
        private readonly ITaskManager _taskManager;

        public InstantUpgradeTask(IToDorfCommand toDorfCommand, IInstantUpgradeCommand instantUpgradeCommand, IUpdateDorfCommand updateDorfCommand, ISwitchVillageCommand switchVillageCommand, ITaskManager taskManager)
        {
            _toDorfCommand = toDorfCommand;
            _instantUpgradeCommand = instantUpgradeCommand;
            _updateDorfCommand = updateDorfCommand;
            _switchVillageCommand = switchVillageCommand;
            _taskManager = taskManager;
        }

        public override async Task<Result> Execute()
        {
            Result result;
            result = await _switchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _toDorfCommand.Execute(AccountId, 1);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _instantUpgradeCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _updateDorfCommand.Execute(AccountId, VillageId);
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