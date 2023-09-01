namespace MainCore.Commands
{
    public interface ICloseBrowserCommand
    {
        Task Execute(int accountId);
    }
}