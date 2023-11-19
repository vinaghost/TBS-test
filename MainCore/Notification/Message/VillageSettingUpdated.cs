using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class VillageSettingUpdated : INotification
    {
        public VillageSettingUpdated(AccountId accountId, VillageId villageId)
        {
            AccountId = accountId;
            VillageId = villageId;
        }

        public AccountId AccountId { get; }
        public VillageId VillageId { get; }
    }
}