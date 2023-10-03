namespace MainCore.Common.Commands
{
    public interface ICloseBrowserCommand
    {
        Task Execute(int accountId);
    }
}