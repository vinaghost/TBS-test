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

        public string GetName()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                SetName().Wait();
            }
            return _name;
        }

        protected string _name;

        protected abstract Task SetName();
    }
}