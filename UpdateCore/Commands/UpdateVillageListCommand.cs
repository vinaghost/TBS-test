using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Models;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;
using UpdateCore.Tasks;

namespace UpdateCore.Commands
{
    public class UpdateVillageListCommand : IUpdateVillageListCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IVillageListParser _villageListParser;
        private readonly IVillageRepository _villageRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly ITaskManager _taskManager;

        public UpdateVillageListCommand(IChromeManager chromeManager, IVillageListParser villageListParser, IVillageRepository villageRepository, IAccountSettingRepository accountSettingRepository, ITaskManager taskManager)
        {
            _chromeManager = chromeManager;
            _villageListParser = villageListParser;
            _villageRepository = villageRepository;
            _accountSettingRepository = accountSettingRepository;
            _taskManager = taskManager;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var listNode = _villageListParser.GetVillages(html);
            if (listNode.Count == 0) return Result.Fail(new Retry("Villages list is empty"));
            var foundVills = new List<Village>();
            foreach (var node in listNode)
            {
                var id = _villageListParser.GetId(node);
                var name = _villageListParser.GetName(node);
                var x = _villageListParser.GetX(node);
                var y = _villageListParser.GetY(node);
                foundVills.Add(new()
                {
                    AccountId = accountId,
                    Id = id,
                    Name = name,
                    X = x,
                    Y = y,
                });
            }

            var newVillages = await _villageRepository.Update(accountId, foundVills);
            if (newVillages.Count > 0)
            {
                var isLoadNewVillage = await _accountSettingRepository.GetBoolSetting(accountId, AccountSettingEnums.IsAutoLoadVillage);
                if (isLoadNewVillage)
                {
                    foreach (var village in newVillages)
                    {
                        _taskManager.Add<UpdateVillageTask>(accountId, village.Id);
                    }
                }
            }
            return Result.Ok();
        }
    }
}