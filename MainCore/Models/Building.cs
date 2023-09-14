using MainCore.Enums;

namespace MainCore.Models
{
    public class Building
    {
        public int Id { get; set; }

        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public bool IsUnderConstruction { get; set; }
        public int Location { get; set; }
        public int VillageId { get; set; }
    }
}