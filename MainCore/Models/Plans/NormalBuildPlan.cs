using MainCore.Enums;

namespace MainCore.Models.Plans
{
    public class NormalBuildPlan
    {
        public int Location { get; set; }
        public int Level { get; set; }
        public BuildingEnums Type { get; set; }
    }
}