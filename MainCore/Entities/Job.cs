using MainCore.Common.Enums;

namespace MainCore.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public int Position { get; set; }
        public JobTypeEnums Type { get; set; }
        public string Content { get; set; }
    }
}