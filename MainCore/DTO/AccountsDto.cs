using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class AccountsDto
    {
        public string Username { get; set; }
        public string Server { get; set; }
        public string Password { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
    }

    [Mapper]
    public partial class AccountsMapper
    {
        public Account Map(AccountsDto dto)
        {
            var account = MapEnity(dto);
            account.Accesses = new List<Access>()
            {
                new()
                {
                    Password = dto.Password,
                    ProxyHost = dto.ProxyHost,
                    ProxyPort = dto.ProxyPort,
                    ProxyUsername = dto.ProxyUsername,
                    ProxyPassword = dto.ProxyPassword,
                },
            };
            return account;
        }

        public partial List<Account> Map(List<AccountsDto> dtos);

        private partial Account MapEnity(AccountsDto dto);
    }
}