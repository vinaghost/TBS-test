using FluentResults;
using MainCore.DTO;

namespace MainCore.Features.Proxy.Errors
{
    public class ProxyNotWork : Error
    {
        public ProxyNotWork(AccessDto access) : base($"Proxy {access.ProxyHost} is not working after 3 tries")
        {
        }
    }
}