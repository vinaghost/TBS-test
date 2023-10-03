namespace MainCore.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Server { get; set; }

        public AccountInfo Info { get; set; }

        public ICollection<AccountSetting> Settings { get; set; }
        public ICollection<Access> Accesses { get; set; }
        public ICollection<Village> Villages { get; set; }

        public ICollection<HeroItem> HeroItems { get; set; }
        public ICollection<FarmList> FarmLists { get; set; }
    }
}