using MainCore.Entities;
using MediatR;

namespace MainCore.Common.Notification
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