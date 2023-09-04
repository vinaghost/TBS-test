using FluentResults;
using MainCore.Enums;

namespace MainCore.Tasks
{
    public abstract class TaskBase
    {
        public StageEnums Stage { get; set; }
        public DateTime ExecuteAt { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public abstract Task<Result> Execute();

        public abstract string GetName();
    }
}