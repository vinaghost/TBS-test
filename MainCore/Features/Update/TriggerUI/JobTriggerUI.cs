using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Features.Update.TriggerUI
{
    public class JobTriggerUI : IRequest
    {
        public int VillageId { get; }

        public JobTriggerUI(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class JobTriggerUIHandler : IRequestHandler<JobTriggerUI>
    {
        private readonly BuildViewModel _buildViewModel;

        public JobTriggerUIHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(JobTriggerUI request, CancellationToken cancellationToken)
        {
            await _buildViewModel.JobUpdate(request.VillageId);
        }
    }
}