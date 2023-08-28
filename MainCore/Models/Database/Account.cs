﻿using Microsoft.EntityFrameworkCore;

namespace MainCore.Models.Database
{
    [Index(nameof(Username), nameof(Server), IsUnique = true)]
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Server { get; set; }

        public AccountInfo Info { get; set; }
        public ICollection<Access> Accesses { get; set; }
    }
}