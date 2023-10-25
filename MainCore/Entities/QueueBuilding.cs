using MainCore.Common.Enums;

namespace MainCore.Entities
{
    public class QueueBuilding
    {
        public int Id { get; set; }
        public VillageId VillageId { get; set; }
        public int Position { get; set; }
        public int Location { get; set; } = -1;
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public DateTime CompleteTime { get; set; }
    }
}