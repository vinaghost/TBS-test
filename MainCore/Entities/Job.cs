using MainCore.Common.Enums;
using StronglyTypedIds;

namespace MainCore.Entities
{
    public class Job
    {
        public JobId Id { get; set; }
        public VillageId VillageId { get; set; }
        public int Position { get; set; }
        public JobTypeEnums Type { get; set; }
        public string Content { get; set; }
    }

    [StronglyTypedId]
    public partial struct JobId
    { }
}