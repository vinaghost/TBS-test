using MainCore.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Entities
{
    [Index(nameof(Type))]
    public class HeroItem
    {
        public int Id { get; set; }
        public HeroItemEnums Type { get; set; }
        public int Amount { get; set; }
        public int AccountId { get; set; }
    }
}