using FluentResults;
using MainCore.DTO;

namespace MainCore.Common.Errors
{
    public class ProxyNotWork : Error
    {
        public ProxyNotWork(AccessDto access) : base($"Proxy {access.ProxyHost} is not working after 3 tries")
        {
        }
    }
}