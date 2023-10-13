using System.Threading.Tasks;

namespace MainCore.UI.Commands
{
    public interface IPauseCommand
    {
        Task Execute(int accountId);
    }
}