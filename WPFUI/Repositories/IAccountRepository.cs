using MainCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFUI.Models.Input;

namespace WPFUI.Repositories
{
    public interface IAccountRepository
    {
        event Func<Task> AccountTableChanged;

        Task Add(AccountInput input);

        Task AddRange(List<AccountsInput> inputs);

        Task Delete(int accountId);

        Task Edit(AccountInput account);

        Task<List<Account>> Get();

        Task<Account> Get(int accountId);
    }
}