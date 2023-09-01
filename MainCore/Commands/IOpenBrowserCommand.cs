namespace MainCore.Commands
{
    public interface IOpenBrowserCommand
    {
        Task Execute(int accountId);
    }
}