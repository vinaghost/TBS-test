using MainCore.Entities;
using Riok.Mapperly.Abstractions;

namespace MainCore.DTO
{
    public class AccessDto
    {
        public int Id { get; set; }
        public string Password { get; set; }

        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string Useragent { get; set; }
        public DateTime LastUsed { get; set; }
    }

    [Mapper]
    public partial class AccessMapper
    {
        public Access Map(int accountId, AccessDto dto)
        {
            var entity = Map(dto);
            entity.AccountId = accountId;
            return entity;
        }

        public partial AccessDto Map(Access dto);

        private partial Access Map(AccessDto dto);
    }
}