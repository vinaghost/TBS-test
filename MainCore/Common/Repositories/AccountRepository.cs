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
        private readonly AppDbContext _context;
        private readonly IUseragentManager _useragentManager;
        private readonly IAccountSettingRepository _accountSettingRepository;

        public AccountRepository(AppDbContext context, IUseragentManager useragentManager, MainCore.Common.Repositories.IAccountSettingRepository accountSettingRepository)
        {
            _context = context;
            _useragentManager = useragentManager;
            _accountSettingRepository = accountSettingRepository;
        }

        public Result Add(AccountDto dto)
        {
            var mapper = new AccountMapper();
            var account = mapper.Map(dto);
            return Add(account);
        }

        public Result Add(Account account)
        {
            var query = _context.Accounts
                .Where(x => x.Username == account.Username
                            && x.Server == account.Server);

            if (query.Any()) return Result.Fail(new AccountExist(account.Username, account.Server));
            foreach (var access in account.Accesses)
            {
                access.Useragent = _useragentManager.Get();
            }
            _context.Add(account);
            _context.SaveChanges();

            _accountSettingRepository.CheckSetting(_context, account.Id);

            return Result.Ok();
        }

        public void AddRange(List<AccountDto> dtos)
        {
            var mapper = new AccountMapper();
            var accounts = mapper.Map(dtos);
            AddRange(accounts);
        }

        public void AddRange(List<AccountsDto> dtos)
        {
            var mapper = new AccountsMapper();
            var accounts = mapper.Map(dtos);
            AddRange(accounts);
        }

        public void AddRange(List<Account> accounts)
        {
            foreach (var account in accounts)
            {
                foreach (var access in account.Accesses)
                {
                    access.Useragent = _useragentManager.Get();
                }
            }
            _context.AddRange(accounts);
            _context.SaveChanges();
            foreach (var account in accounts)
            {
                _accountSettingRepository.CheckSetting(_context, account.Id);
            }
        }

        public List<AccountDto> Get()
        {
            var accounts = _context.Accounts
                .AsQueryable()
                .ProjectToDto()
                .ToList();
            return accounts;
        }

        public AccountDto Get(int accountId)
        {
            var account = _context.Accounts
                .Find(accountId);
            _context.Entry(account)
               .Collection(x => x.Accesses)
               .Load();
            var mapper = new AccountMapper();
            return mapper.Map(account);
        }

        public void Edit(AccountDto dto)
        {
            var mapper = new AccountMapper();
            var account = mapper.Map(dto);
            Edit(account);
        }

        public void Edit(Account account)
        {
            var accessIds = account.Accesses
                .Where(x => x.AccountId == account.Id)
                .Select(x => x.Id)
                .ToList();

            var oldAccessIds = accessIds
                .Except(account.Accesses.Select(x => x.Id));

            _context.Accesses
               .Where(x => oldAccessIds.Contains(x.Id))
               .ExecuteDelete();

            _context.Update(account);
            _context.SaveChanges();
        }

        public void Delete(int accountId)
        {
            _context.Accounts
               .Where(x => x.Id == accountId)
               .ExecuteDelete();
        }
    }
}