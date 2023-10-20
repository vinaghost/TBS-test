using MediatR;

namespace MainCore.Common.Notification
{
    public class JobUpdated : INotification
    {
        public int VillageId { get; }

        public JobUpdated(int villageId)
        {
            VillageId = villageId;
        }
    }
}