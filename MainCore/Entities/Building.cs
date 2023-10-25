using MainCore.Common.Enums;

namespace MainCore.Entities
{
    public class Building
    {
        public int Id { get; set; }

        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public bool IsUnderConstruction { get; set; }
        public int Location { get; set; }
        public VillageId VillageId { get; set; }
    }
}