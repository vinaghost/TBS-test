using FluentResults;
using LoginCore.Commands;
using MainCore.Errors;
using MainCore.Tasks;

namespace LoginCore.Tasks
{
    public sealed class LoginTask : AccountTask
    {
        private readonly ILoginCommand _loginCommand;

        public LoginTask(ILoginCommand loginCommand)
        {
            _loginCommand = loginCommand;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            var result = await _loginCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public override string GetName()
        {
            return "Login task";
        }
    }
}