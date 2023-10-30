using MainCore.Common.Enums;
using StronglyTypedIds;

namespace MainCore.Entities
{
    public class Building
    {
        public BuildingId Id { get; set; }

        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public bool IsUnderConstruction { get; set; }
        public int Location { get; set; }
        public VillageId VillageId { get; set; }
    }

    [StronglyTypedId]
    public partial struct BuildingId
    { }
}