using MainCore.Common.Commands;
using MainCore.Common.Enums;
using MainCore.Common.Repositories;
using MainCore.Features.Login.Tasks;
using MainCore.Features.UpgradeBuilding.Tasks;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.UI.Commands
{
    public class LoginCommand : IRequest
    {
        public int AccountId { get; }

        public LoginCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand>
    {
        private readonly ITaskManager _taskManager;
        private readonly ITimerManager _timerManager;
        private readonly IVillageRepository _villageRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IMediator _mediator;

        public LoginCommandHandler(ITaskManager taskManager, ITimerManager timerManager, IMediator mediator, IVillageRepository villageRepository, IJobRepository jobRepository)
        {
            _taskManager = taskManager;
            _timerManager = timerManager;
            _mediator = mediator;
            _villageRepository = villageRepository;
            _jobRepository = jobRepository;
        }

        public async Task Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            _taskManager.SetStatus(accountId, StatusEnums.Starting);
            await _mediator.Send(new OpenBrowserCommand(accountId), cancellationToken);

            _taskManager.AddOrUpdate<LoginTask>(accountId, first: true);
            await AddUpgradeBuildingTask(accountId);

            _timerManager.Start(accountId);
            _taskManager.SetStatus(accountId, StatusEnums.Online);
        }

        private async Task AddUpgradeBuildingTask(int accountId)
        {
            var villages = await _villageRepository.GetAll(accountId);

            foreach (var village in villages)
            {
                var countJob = await _jobRepository.CountBuildingJob(village.Id);
                if (countJob > 0)
                {
                    _taskManager.AddOrUpdate<UpgradeBuildingTask>(accountId, village.Id);
                }
            }
        }
    }
}