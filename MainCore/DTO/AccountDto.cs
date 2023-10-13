using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Server { get; set; }
        public List<AccessDto> Accesses { get; set; }
    }

    [Mapper]
    public partial class AccountMapper
    {
        public partial Account Map(AccountDto dto);

        public partial List<Account> Map(List<AccountDto> dtos);

        public partial AccountDto Map(Account entity);
    }

    [Mapper]
    public static partial class AccountStaticMapper
    {
        public static partial IQueryable<AccountDto> ProjectToDto(this IQueryable<Account> entities);
    }
}