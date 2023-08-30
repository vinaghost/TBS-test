namespace MainCore.Commands
{
    public interface IAccountCommand
    {
        Task Execute(int accountId);
    }
}