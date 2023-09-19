namespace MainCore.Models
{
    public class Storage
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public long Wood { get; set; }
        public long Clay { get; set; }
        public long Iron { get; set; }
        public long Crop { get; set; }
        public long Warehouse { get; set; }
        public long Granary { get; set; }
        public long FreeCrop { get; set; }
    }
}