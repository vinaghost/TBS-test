namespace LoginCore.Commands
{
    public interface ILoginCommand
    {
        Task Execute(int accountId);
    }
}