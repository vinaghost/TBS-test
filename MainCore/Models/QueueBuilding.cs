using MainCore.Enums;

namespace MainCore.Models
{
    public class QueueBuilding
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public int Position { get; set; }
        public int Location { get; set; } = -1;
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public DateTime CompleteTime { get; set; }
    }
}