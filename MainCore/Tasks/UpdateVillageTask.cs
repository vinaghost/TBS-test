using FluentResults;
using MainCore.Commands;
using MainCore.Common.Errors;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Repositories;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class UpdateVillageTask : VillageTask
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfCommand _unitOfCommand;
        private readonly IUnitOfRepository _unitOfRepository;

        public UpdateVillageTask(IChromeManager chromeManager, IUnitOfCommand unitOfCommand, IUnitOfRepository unitOfRepository)
        {
            _chromeManager = chromeManager;
            _unitOfCommand = unitOfCommand;
            _unitOfRepository = unitOfRepository;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return new Cancel();
            Result result;
            result = _unitOfCommand.SwitchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var chromeBrowser = _chromeManager.Get(AccountId);
            var url = chromeBrowser.CurrentUrl;
            if (url.Contains("dorf1"))
            {
                result = await _unitOfCommand.UpdateDorfCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _unitOfCommand.ToDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else if (url.Contains("dorf2"))
            {
                result = await _unitOfCommand.UpdateDorfCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _unitOfCommand.ToDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else
            {
                result = _unitOfCommand.ToDorfCommand.Execute(AccountId, 2);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = _unitOfCommand.ToDorfCommand.Execute(AccountId, 1);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _unitOfCommand.UpdateDorfCommand.Execute(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }

            return Result.Ok();
        }

        protected override void SetName()
        {
            var village = _unitOfRepository.VillageRepository.GetVillageName(VillageId);
            _name = $"Update buildings in {village}";
        }
    }
}