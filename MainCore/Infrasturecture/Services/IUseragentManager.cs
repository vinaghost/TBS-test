namespace MainCore.Infrasturecture.Services
{
    public interface IUseragentManager : IDisposable
    {
        string Get();

        Task Load();
    }
}