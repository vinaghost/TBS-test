using FluentResults;
using LoginCore.Commands;
using MainCore.Tasks;
using Splat;

namespace LoginCore.Tasks
{
    public class LoginTask : AccountTask
    {
        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Ok();
            var loginCommand = Locator.Current.GetService<ILoginCommand>();
            await loginCommand.Execute(AccountId);
            return Result.Ok();
        }
    }
}