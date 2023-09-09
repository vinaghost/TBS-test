using System.Threading.Tasks;

namespace WPFUI.Commands
{
    public interface ILoginCommand
    {
        Task Execute(int accountId);
    }
}