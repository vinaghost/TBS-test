using MainCore.Enums;

namespace MainCore.Models
{
    public class VillageSetting
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public VillageSettingEnums Setting { get; set; }
        public int Value { get; set; }
    }
}