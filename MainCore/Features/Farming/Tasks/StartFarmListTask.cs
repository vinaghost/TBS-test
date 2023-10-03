using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Repositories;
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
        private readonly IFarmListRepository _farmListRepository;
        private readonly ISendFarmListCommand _sendFarmListCommand;
        private readonly ISendAllFarmListCommand _sendAllFarmListCommand;

        public StartFarmListTask(IToFarmListPageCommand toFarmListPageCommand, IAccountSettingRepository accountSettingRepository, ISendFarmListCommand sendFarmListCommand, ISendAllFarmListCommand sendAllFarmListCommand, IFarmListRepository farmListRepository)

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
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            var useStartAllButton = await _accountSettingRepository.GetBoolSetting(AccountId, AccountSettingEnums.UseStartAllButton);
            if (useStartAllButton)
            {
                result = await _sendAllFarmListCommand.Execute(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            else
            {
                var farmLists = await _farmListRepository.GetActiveFarmLists(AccountId);
                if (farmLists.Count == 0) return Result.Fail(new Skip("No farmlist is active"));

                var clickDelay = await _accountSettingRepository.GetSetting(AccountId, AccountSettingEnums.ClickDelayMin, AccountSettingEnums.ClickDelayMax);

                foreach (var farmList in farmLists)
                {
                    result = await _sendFarmListCommand.Execute(AccountId, farmList);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                    await Task.Delay(clickDelay);
                }
            }
            return Result.Ok();
        }

        protected override Task SetName()
        {
            _name = "Start farm list";
            return Task.CompletedTask;
        }
    }
}