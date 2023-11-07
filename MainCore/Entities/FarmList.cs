using StronglyTypedIds;

namespace MainCore.Entities
{
    public class FarmList
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    [StronglyTypedId]
    public partial struct FarmListId
    { }
}