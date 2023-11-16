using FluentResults;
using MainCore.Commands.General;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Repositories;

namespace MainCore.Tasks
{
    public class SleepTask : AccountTask
    {
        private readonly IChooseAccessCommand _chooseAccessCommand;
        private readonly ISleepCommand _sleepCommand;
        private readonly IWorkCommand _workCommand;
        private readonly IUnitOfRepository _unitOfRepository;

        public SleepTask(IChooseAccessCommand chooseAccessCommand, ISleepCommand sleepCommand, IWorkCommand workCommand, IUnitOfRepository unitOfRepository)
        {
            _chooseAccessCommand = chooseAccessCommand;
            _sleepCommand = sleepCommand;
            _workCommand = workCommand;
            _unitOfRepository = unitOfRepository;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            var sleepTimeMinutes = _unitOfRepository.AccountSettingRepository.GetByName(AccountId, AccountSettingEnums.SleepTimeMin, AccountSettingEnums.SleepTimeMax);
            Result result;
            result = await _sleepCommand.Execute(AccountId, TimeSpan.FromMinutes(sleepTimeMinutes), CancellationToken);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            var resultAccess = await _chooseAccessCommand.Execute(AccountId, false);
            if (resultAccess.IsFailed) return Result.Fail(resultAccess.Errors).WithError(new TraceMessage(TraceMessage.Line()));
            var access = resultAccess.Value;
            result = await Task.Run(() => _workCommand.Execute(AccountId, access));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        protected override void SetName()
        {
            _name = "Sleep task";
        }
    }
}