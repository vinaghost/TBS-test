using StronglyTypedIds;

namespace MainCore.Entities
{
    public class FarmList
    {
        public FarmListId Id { get; set; }

        public AccountId AccountId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    [StronglyTypedId]
    public partial struct FarmListId
    { }
}