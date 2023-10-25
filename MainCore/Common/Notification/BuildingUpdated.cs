using MainCore.Entities;
using MediatR;

namespace MainCore.Common.Notification
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