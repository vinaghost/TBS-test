using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Features.Update.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
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

        public async Task<Result> Execute(IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.Html;

            var dto = _stockBarParser.Get(html);
            var mapper = new StorageMapper();
            var storage = mapper.Map(villageId, dto);
            await _storageRepository.Update(villageId, storage);
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(chromeBrowser, villageId);
        }
    }
}