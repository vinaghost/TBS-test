using System.Threading.Tasks;

namespace WPFUI.Commands
{
    public interface IRestartCommand
    {
        Task Execute(int accountId);
    }
}