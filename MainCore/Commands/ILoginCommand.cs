namespace MainCore.Commands
{
    public interface ILoginCommand
    {
        Task Execute(int accountId);
    }
}