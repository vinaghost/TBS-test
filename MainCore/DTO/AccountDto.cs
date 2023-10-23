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

        public static AccountDto Create(string username, string server, string password)
        {
            return new AccountDto()
            {
                Username = username,
                Server = server,
                Accesses = new()
                {
                    new AccessDto()
                    {
                        Password = password,
                    },
                },
            };
        }

        public static AccountDto Create(string username, string server, string password, string proxyHost, int proxyPort)
        {
            var dto = Create(username, server, password);
            var access = dto.Accesses[0];
            access.ProxyHost = proxyHost;
            access.ProxyPort = proxyPort;
            return dto;
        }

        public static AccountDto Create(string username, string server, string password, string proxyHost, int proxyPort, string proxyUsername, string proxyPassword)
        {
            var dto = Create(username, server, password, proxyHost, proxyPort);
            var access = dto.Accesses[0];
            access.ProxyUsername = proxyUsername;
            access.ProxyPassword = proxyPassword;
            return dto;
        }
    }

    [Mapper]
    public partial class AccountMapper
    {
        public partial Account Map(AccountDto dto);

        public partial AccountDto Map(Account entity);
    }

    [Mapper]
    public static partial class AccountStaticMapper
    {
        public static partial IQueryable<AccountDto> ProjectToDto(this IQueryable<Account> entities);
    }
}