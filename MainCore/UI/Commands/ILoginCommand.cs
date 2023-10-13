using System.Threading.Tasks;

namespace MainCore.UI.Commands
{
    public interface ILoginCommand
    {
        Task Execute(int accountId);
    }
}