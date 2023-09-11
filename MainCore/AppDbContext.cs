using MainCore.Models;
using Microsoft.EntityFrameworkCore;

namespace MainCore
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountInfo> AccountsInfo { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<AccountSetting> AccountsSetting { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<Building> Buildings { get; set; }
    }
}