using System.Threading.Tasks;

namespace WPFUI.Commands
{
    public interface ILogoutCommand
    {
        Task Execute(int accountId);
    }
}