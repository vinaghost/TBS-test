using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class StorageUpdated : INotification
    {
        public VillageId VillageId { get; }

        public StorageUpdated(VillageId villageId)
        {
            VillageId = villageId;
        }
    }
}