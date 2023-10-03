using Serilog;
using Serilog.Events;

namespace MainCore.Infrasturecture.Services
{
    public interface ILogService
    {
        LinkedList<LogEvent> GetLog(int accountId);
        ILogger GetLogger(int accountId);
        void Init();
        void Shutdown();
    }
}