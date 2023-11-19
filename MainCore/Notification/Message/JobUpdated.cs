using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class JobUpdated : INotification
    {
        public VillageId VillageId { get; }

        public JobUpdated(VillageId villageId)
        {
            VillageId = villageId;
        }
    }
}