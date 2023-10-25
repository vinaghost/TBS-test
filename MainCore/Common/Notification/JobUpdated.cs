using MainCore.Entities;
using MediatR;

namespace MainCore.Common.Notification
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