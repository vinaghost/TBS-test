using MainCore.Entities;
using MediatR;

namespace MainCore.Notification
{
    public class VillageSettingUpdated : INotification
    {
        public VillageId VillageId { get; }

        public VillageSettingUpdated(VillageId villageId)
        {
            VillageId = villageId;
        }
    }
}