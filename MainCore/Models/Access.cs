﻿namespace MainCore.Models
{
    public class Access
    {
        public int Id { get; set; }

        public string Password { get; set; }

        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string Useragent { get; set; }
        public DateTime LastUsed { get; set; }

        public int AccountId { get; set; }
    }
}