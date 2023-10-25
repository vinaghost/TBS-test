using MainCore.Entities;
using MediatR;

namespace MainCore.Common.Notification
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