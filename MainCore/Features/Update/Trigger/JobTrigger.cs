using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Features.Update.Trigger
{
    public class JobTrigger : INotification
    {
        public int VillageId { get; }

        public JobTrigger(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class JobTriggerHandler : INotificationHandler<JobTrigger>
    {
        private readonly BuildViewModel _buildViewModel;

        public JobTriggerHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(JobTrigger request, CancellationToken cancellationToken)
        {
            await _buildViewModel.JobUpdate(request.VillageId);
        }
    }
}