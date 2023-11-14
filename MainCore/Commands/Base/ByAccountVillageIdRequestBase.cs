using MainCore.Entities;

namespace MainCore.Commands.Base
{
    public class ByAccountVillageIdRequestBase : ByAccountIdRequestBase
    {
        public VillageId VillageId { get; }

        public ByAccountVillageIdRequestBase(AccountId accountId, VillageId villageId) : base(accountId)
        {
            VillageId = villageId;
        }
    }
}