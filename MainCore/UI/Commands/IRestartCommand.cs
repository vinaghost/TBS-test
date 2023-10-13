using System.Threading.Tasks;

namespace MainCore.UI.Commands
{
    public interface IRestartCommand
    {
        Task Execute(int accountId);
    }
}