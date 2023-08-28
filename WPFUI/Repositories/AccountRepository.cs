using MainCore;
using MainCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPFUI.Models.Input;

namespace WPFUI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public event Func<Task> AccountTableChanged;

        public AccountRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Add(AccountInput input)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var account = new Account
                {
                    Username = input.Username,
                    Server = input.Server,
                    Accesses = input.Accesses.Select(x => x.GetAccess()).ToList(),
                    Info = new()
                };

                await context.AddAsync(account);
                await context.SaveChangesAsync();
            }
            await AccountTableChanged?.Invoke();
        }

        public async Task<List<Account>> Get()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Accounts.ToListAsync();
        }

        public async Task Delete(int accountId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                await context.Accounts.Where(x => x.Id == accountId).ExecuteDeleteAsync();
            }
            await AccountTableChanged?.Invoke();
        }
    }
}