using MainCore.Enums;

namespace MainCore.Models
{
    public class AccountInfo
    {
        public int Id { get; set; }
        public TribeEnums Tribe { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public bool HasPlusAccount { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}