using MediatR;

namespace MainCore.Common.Notification
{
    public class StorageUpdated : INotification
    {
        public int VillageId { get; }

        public StorageUpdated(int villageId)
        {
            VillageId = villageId;
        }
    }
}