using FarmingCore.Commands;
using FluentResults;
using MainCore.Errors;
using MainCore.Tasks;
using UpdateCore.Commands;

namespace FarmingCore.Tasks
{
    public class UpdateFarmListTask : AccountTask
    {
        private readonly IToFarmListPageCommand _toFarmListPageCommand;
        private readonly IUpdateFarmListCommand _updateFarmListCommand;

        public UpdateFarmListTask(IToFarmListPageCommand toFarmListPageCommand, IUpdateFarmListCommand updateFarmListCommand)
        {
            _toFarmListPageCommand = toFarmListPageCommand;
            _updateFarmListCommand = updateFarmListCommand;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = await _toFarmListPageCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _updateFarmListCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        protected override Task SetName()
        {
            _name = "Update farm lists";
            return Task.CompletedTask;
        }
    }
}