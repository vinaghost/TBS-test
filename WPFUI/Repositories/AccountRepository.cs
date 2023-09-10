using MainCore;
using MainCore.Models;
using MainCore.Services;
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
        private readonly IUseragentManager _useragentManager;

        public event Func<Task> AccountTableChanged;

        public AccountRepository(IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager)
        {
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
        }

        public async Task Add(AccountInput input)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var account = input.GetAccount();
                foreach (var access in account.Accesses)
                {
                    access.Useragent = _useragentManager.Get();
                }

                await context.AddAsync(account);
                await context.SaveChangesAsync();
            }
            await AccountTableChanged?.Invoke();
        }

        public async Task AddRange(List<AccountsInput> inputs)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var accounts = new List<Account>();
                foreach (var input in inputs)
                {
                    var account = input.GetAccount();
                    foreach (var access in account.Accesses)
                    {
                        access.Useragent = _useragentManager.Get();
                    }
                    accounts.Add(account);
                }

                await context.AddRangeAsync(accounts);
                await context.SaveChangesAsync();
            }
            if (AccountTableChanged is not null)
            {
                await AccountTableChanged();
            }
        }

        public async Task<List<Account>> Get()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Accounts.ToListAsync();
        }

        public async Task<Account> Get(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var account = await context.Accounts.FindAsync(accountId);
            await context.Entry(account).Collection(x => x.Accesses).LoadAsync();
            return account;
        }

        public async Task Edit(AccountInput input)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var account = await context.Accounts.FindAsync(input.Id);
                await context.Entry(account).Collection(x => x.Accesses).LoadAsync();

                account.Username = input.Username;
                account.Server = input.Server;

                var accessAdded = new List<int>();
                foreach (var access in input.Accesses)
                {
                    var accountAccess = account.Accesses.FirstOrDefault(x => x.Id == access.Id);
                    if (accountAccess is null)
                    {
                        accountAccess = access.GetAccess();
                        accountAccess.Useragent = _useragentManager.Get();
                        account.Accesses.Add(accountAccess);
                    }
                    else
                    {
                        access.CopyTo(accountAccess);
                    }

                    accessAdded.Add(access.Id);
                }
                var accessRemove = account.Accesses.Where(x => !accessAdded.Contains(x.Id)).ToList();
                foreach (var access in accessRemove)
                {
                    account.Accesses.Remove(access);
                }
                context.Update(account);
                await context.SaveChangesAsync();
            }
            if (AccountTableChanged is not null)
            {
                await AccountTableChanged();
            }
        }

        public async Task Delete(int accountId)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                await context.Accounts.Where(x => x.Id == accountId).ExecuteDeleteAsync();
            }
            if (AccountTableChanged is not null)
            {
                await AccountTableChanged();
            }
        }
    }
}