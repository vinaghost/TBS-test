using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Common.Requests
{
    public class JobUpdate : IRequest
    {
        public int VillageId { get; }

        public JobUpdate(int villageId)
        {
            VillageId = villageId;
        }
    }

    public class JobChangeHandler : IRequestHandler<JobUpdate>
    {
        private readonly BuildViewModel _buildViewModel;

        public JobChangeHandler(BuildViewModel buildViewModel)
        {
            _buildViewModel = buildViewModel;
        }

        public async Task Handle(JobUpdate request, CancellationToken cancellationToken)
        {
            await _buildViewModel.JobUpdate(request.VillageId);
        }
    }
}