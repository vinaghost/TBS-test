using MainCore.DTO;
using RestSharp;

namespace MainCore.Infrasturecture.Services
{
    public interface IRestClientManager
    {
        RestClient Get(AccessDto access);
        void Shutdown();
    }
}