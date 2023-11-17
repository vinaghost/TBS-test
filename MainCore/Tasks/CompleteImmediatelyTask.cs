using FluentResults;
using MainCore.Commands;
using MainCore.Commands.Special;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;
using MainCore.Services;
using MediatR;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class CompleteImmediatelyTask : VillageTask
    {
        private readonly IUnitOfCommand _unitOfCommand;
        private readonly ITaskManager _taskManager;
        private readonly IMediator _mediator;
        private readonly IUnitOfRepository _unitOfRepository;

        public CompleteImmediatelyTask(ITaskManager taskManager, IUnitOfCommand unitOfCommand, IMediator mediator, IUnitOfRepository unitOfRepository)
        {
            _taskManager = taskManager;
            _unitOfCommand = unitOfCommand;
            _mediator = mediator;
            _unitOfRepository = unitOfRepository;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = _unitOfCommand.SwitchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _unitOfCommand.ToDorfCommand.Execute(AccountId, 1);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _mediator.Send(new CompleteImmediatelyCommand(AccountId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _unitOfCommand.UpdateDorfCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            TriggerUpgradeBuilding();
            return Result.Ok();
        }

        protected override void SetName()
        {
            var villageName = _unitOfRepository.VillageRepository.GetVillageName(VillageId);
            _name = $"Complete immediately in {villageName}";
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