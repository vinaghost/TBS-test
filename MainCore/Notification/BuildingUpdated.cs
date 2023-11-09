using MainCore.Entities;
using MediatR;

namespace MainCore.Notification
{
    public class BuildingUpdated : INotification
    {
        public VillageId VillageId { get; }

        public BuildingUpdated(VillageId villageId)
        {
            VillageId = villageId;
        }
    }
}