using MainCore.Entities;
using Serilog;
using Serilog.Events;

namespace MainCore.Infrasturecture.Services
{
    public interface ILogService
    {
        LinkedList<LogEvent> GetLog(AccountId accountId);

        ILogger GetLogger(AccountId accountId);

        void Load();

        void Shutdown();
    }
}