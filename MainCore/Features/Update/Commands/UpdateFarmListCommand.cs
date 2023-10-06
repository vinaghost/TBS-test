using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Update.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateFarmListCommand : IUpdateFarmListCommand
    {
        private readonly IFarmListRepository _farmListRepository;
        private readonly IChromeManager _chromeManager;
        private readonly IFarmListParser _farmListParser;

        public UpdateFarmListCommand(IFarmListRepository farmListRepository, IChromeManager chromeManager, IFarmListParser farmListParser)
        {
            _farmListRepository = farmListRepository;
            _chromeManager = chromeManager;
            _farmListParser = farmListParser;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var dtos = _farmListParser.Get(html);
            var mapper = new FarmListMapper();

            var farmLists = new List<FarmList>();

            foreach (var dto in dtos)
            {
                var farmList = mapper.Map(accountId, dto);
                farmLists.Add(farmList);
            }

            await _farmListRepository.Update(accountId, farmLists);
            return Result.Ok();
        }
    }
}