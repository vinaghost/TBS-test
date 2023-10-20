using MediatR;

namespace MainCore.Common.Notification
{
    public class BuildingUpdated : INotification
    {
        public int VillageId { get; }

        public BuildingUpdated(int villageId)
        {
            VillageId = villageId;
        }
    }
}