using MainCore.Entities;

namespace MainCore.Commands.Base
{
    public class ByVillageIdRequestBase
    {
        public VillageId VillageId { get; }

        public ByVillageIdRequestBase(VillageId villageId)
        {
            VillageId = villageId;
        }
    }
}