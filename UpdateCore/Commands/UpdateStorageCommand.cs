using FluentResults;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;

namespace UpdateCore.Commands
{
    public class UpdateStorageCommand : IUpdateStorageCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IStorageRepository _storageRepository;
        private readonly IStockBarParser _stockBarParser;

        public UpdateStorageCommand(IChromeManager chromeManager, IStorageRepository storageRepository, IStockBarParser stockBarParser)
        {
            _chromeManager = chromeManager;
            _storageRepository = storageRepository;
            _stockBarParser = stockBarParser;
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var storage = _stockBarParser.GetStorage(html);
            storage.VillageId = villageId;
            await _storageRepository.Update(villageId, storage);
            return Result.Ok();
        }
    }
}