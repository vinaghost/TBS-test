using FluentResults;
using MainCore.Common.Errors.Database;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Repositories
{
    [RegisterAsSingleton]
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IAccountSettingRepository _accountSettingRepository;

        public event Func<Task> AccountTableChanged;

        public AccountRepository(IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, MainCore.Common.Repositories.IAccountSettingRepository accountSettingRepository)
        {
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _accountSettingRepository = accountSettingRepository;
        }

        public async Task Add(AccountDto dto)
        {
            var mapper = new AccountMapper();
            var account = mapper.Map(dto);
            await Add(account);
        }

        public async Task<Result> Add(Account account)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var query = context.Accounts
                    .Where(x => x.Username == account.Username
                                && x.Server == account.Server);
                if (query.Any()) return Result.Fail(new AccountExist(account.Username, account.Server));
                foreach (var access in account.Accesses)
                {
                    access.Useragent = _useragentManager.Get();
                }
                await context.AddAsync(account);
                await context.SaveChangesAsync();

                await _accountSettingRepository.CheckSetting(account.Id, context);
            }
            if (AccountTableChanged is not null)
            {
                await AccountTableChanged();
            }
            return Result.Ok();
        }

        public async Task AddRange(List<AccountDto> dtos)
        {
            var mapper = new AccountMapper();
            var accounts = mapper.Map(dtos);
            await AddRange(accounts);
        }

        public async Task AddRange(List<AccountsDto> dtos)
        {
            var mapper = new AccountsMapper();
            var accounts = mapper.Map(dtos);
            await AddRange(accounts);
        }

        public async Task AddRange(List<Account> accounts)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                foreach (var account in accounts)
                {
                    foreach (var access in account.Accesses)
                    {
                        access.Useragent = _useragentManager.Get();
                    }
                }
                await context.AddRangeAsync(accounts);
                await context.SaveChangesAsync();
                foreach (var account in accounts)
                {
                    await _accountSettingRepository.CheckSetting(account.Id, context);
                }
            }
            if (AccountTableChanged is not null)
            {
                await AccountTableChanged();
            }
        }

        public async Task<List<AccountDto>> Get()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var accounts = await context.Accounts
                .AsQueryable()
                .ProjectToDto()
                .ToListAsync();
            return accounts;
        }

        public async Task<AccountDto> Get(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var account = await context.Accounts
                .FindAsync(accountId);
            await context.Entry(account)
                .Collection(x => x.Accesses)
                .LoadAsync();
            var mapper = new AccountMapper();
            return mapper.Map(account);
        }

        public async Task Edit(AccountDto dto)
        {
            var mapper = new AccountMapper();
            var account = mapper.Map(dto);
            await Edit(account);
        }

        public async Task Edit(Account account)
        {
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var accessIds = account.Accesses
                    .Where(x => x.AccountId == account.Id)
                    .Select(x => x.Id)
                    .ToList();

                var oldAccessIds = accessIds.Except(account.Accesses.Select(x => x.Id));

                await context.Accesses
                    .Where(x => oldAccessIds.Contains(x.Id))
                    .ExecuteDeleteAsync();

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
                await context.Accounts
                    .Where(x => x.Id == accountId)
                    .ExecuteDeleteAsync();
            }

            if (AccountTableChanged is not null)
            {
                await AccountTableChanged();
            }
        }
    }
}