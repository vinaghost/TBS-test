using MainCore.Common.MediatR;
using MainCore.Entities;
using MainCore.Repositories;
using MainCore.Services;
using MainCore.Tasks;
using MediatR;

namespace MainCore.Commands.Trigger
{
    public class TriggerUpgradeBuildingTask : ByAccountVillageIdBase, IRequest
    {
        public TriggerUpgradeBuildingTask(AccountId accountId, VillageId villageId) : base(accountId, villageId)
        {
        }
    }

    public class TriggerUpgradeBuildingTaskHandler : IRequestHandler<TriggerUpgradeBuildingTask>
    {
        private readonly ITaskManager _taskManager;
        private readonly IUnitOfRepository _unitOfRepository;

        public TriggerUpgradeBuildingTaskHandler(ITaskManager taskManager, IUnitOfRepository unitOfRepository)
        {
            _taskManager = taskManager;
            _unitOfRepository = unitOfRepository;
        }

        public async Task Handle(TriggerUpgradeBuildingTask request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var villageId = request.VillageId;
            var countBuildJob = await Task.Run(() => _unitOfRepository.JobRepository.CountBuildingJob(villageId));
            if (countBuildJob == 0) return;
            _taskManager.AddOrUpdate<UpgradeBuildingTask>(accountId, villageId);
        }
    }
}