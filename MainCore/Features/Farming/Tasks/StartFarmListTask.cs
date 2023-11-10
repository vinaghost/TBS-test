using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Repositories;
using MainCore.Common.Tasks;
using MainCore.Features.Farming.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Farming.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class StartFarmListTask : AccountTask
    {
        private readonly IToFarmListPageCommand _toFarmListPageCommand;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IFarmRepository _farmListRepository;
        private readonly ISendFarmListCommand _sendFarmListCommand;
        private readonly ISendAllFarmListCommand _sendAllFarmListCommand;

        public StartFarmListTask(IToFarmListPageCommand toFarmListPageCommand, IAccountSettingRepository accountSettingRepository, ISendFarmListCommand sendFarmListCommand, ISendAllFarmListCommand sendAllFarmListCommand, IFarmRepository farmListRepository)
        {
            _toFarmListPageCommand = toFarmListPageCommand;
            _accountSettingRepository = accountSettingRepository;
            _sendFarmListCommand = sendFarmListCommand;
            _sendAllFarmListCommand = sendAllFarmListCommand;
            _farmListRepository = farmListRepository;
        }

        public override async Task<Result> Execute()
        {
            Result result;
            result = await _toFarmListPageCommand.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            var useStartAllButton = _accountSettingRepository.GetBooleanByName(AccountId, AccountSettingEnums.UseStartAllButton);
            if (useStartAllButton)
            {
                result = _sendAllFarmListCommand.Execute(AccountId);
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }
            else
            {
                var farmLists = _farmListRepository.GetActive(AccountId);
                if (farmLists.Count == 0) return Result.Fail(new Skip("No farmlist is active"));

                var clickDelay = _accountSettingRepository.GetByName(AccountId, AccountSettingEnums.ClickDelayMin, AccountSettingEnums.ClickDelayMax);

                foreach (var farmList in farmLists)
                {
                    result = _sendFarmListCommand.Execute(AccountId, farmList);
                    if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

                    await Task.Delay(clickDelay);
                }
            }
            return Result.Ok();
        }

        protected override void SetName()
        {
            _name = "Start farm list";
        }
    }
}