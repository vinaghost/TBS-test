using System.Threading.Tasks;

namespace MainCore.UI.Commands
{
    public interface ILogoutCommand
    {
        Task Execute(int accountId);
    }
}