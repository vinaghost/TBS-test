using MainCore.Common.Enums;

namespace WPFUI.Models.Output
{
    public class BuildingItem
    {
        public int Id { get; set; }
        public int Location { get; set; }
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public int QueueLevel { get; set; } = 0;
        public int JobLevel { get; set; } = 0;
    }
}