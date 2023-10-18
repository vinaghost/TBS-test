using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Features.Login.Commands;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Login.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
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
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await _updateVillageListCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        protected override void SetName()
        {
            _name = "Login task";
        }
    }
}