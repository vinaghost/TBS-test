using MediatR;

namespace MainCore.Common.Notification
{
    internal class QueueBuildingUpdated : INotification
    {
        public int VillageId { get; }

        public QueueBuildingUpdated(int villageId)
        {
            VillageId = villageId;
        }
    }
}