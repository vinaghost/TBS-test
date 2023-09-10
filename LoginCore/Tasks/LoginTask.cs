using FluentResults;
using LoginCore.Commands;
using MainCore.Errors;
using MainCore.Tasks;
using UpdateCore.Commands;

namespace LoginCore.Tasks
{
    public sealed class LoginTask : AccountTask
    {
        private readonly ILoginCommand _loginCommand;
        private readonly IUpdateVillageListCommand _updateVillageListCommand;

        public LoginTask(ILoginCommand loginCommand, IUpdateVillageListCommand updateVillageListCommand)
        {
            _loginCommand = loginCommand;
            _updateVillageListCommand = updateVillageListCommand;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = await _loginCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _updateVillageListCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public override string GetName()
        {
            return "Login task";
        }
    }
}