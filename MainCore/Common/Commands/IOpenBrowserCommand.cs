namespace MainCore.Common.Commands
{
    public interface IOpenBrowserCommand
    {
        Task Execute(int accountId);
    }
}