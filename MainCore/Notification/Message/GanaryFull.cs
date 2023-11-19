using MainCore.Commands.Base;
using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    public class GanaryFull : ByAccountVillageIdRequestBase, INotification
    {
        public GanaryFull(AccountId accountId, VillageId villageId) : base(accountId, villageId)
        {
        }
    }
}