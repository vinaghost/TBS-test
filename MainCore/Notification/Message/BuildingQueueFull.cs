using MainCore.Commands.Base;
using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class BuildingQueueFull : ByAccountVillageIdRequestBase, INotification
    {
        public BuildingQueueFull(AccountId accountId, VillageId villageId) : base(accountId, villageId)
        {
        }
    }
}