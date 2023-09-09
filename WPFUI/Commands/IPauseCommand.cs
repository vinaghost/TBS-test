using System.Threading.Tasks;

namespace WPFUI.Commands
{
    public interface IPauseCommand
    {
        Task Execute(int accountId);
    }
}