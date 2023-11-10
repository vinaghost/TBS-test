using MainCore.Parsers;
using MainCore.Repositories;

namespace MainCore.Common
{
    public interface IUnitOfWork
    {
        IAccountInfoParser AccountInfoParser { get; }
        IAccountInfoRepository AccountInfoRepository { get; }
        IAccountRepository AccountRepository { get; }
        IAccountSettingRepository AccountSettingRepository { get; }
        IBuildingParser BuildingParser { get; }
        IBuildingRepository BuildingRepository { get; }
        ICompleteImmediatelyParser CompleteImmediatelyParser { get; }
        IFarmRepository FarmRepository { get; }
        IFarmParser FarmParser { get; }
        IFieldParser FieldParser { get; }
        IHeroItemRepository HeroItemRepository { get; }
        IHeroParser HeroParser { get; }
        IInfrastructureParser InfrastructureParser { get; }
        IJobRepository JobRepository { get; }
        ILoginPageParser LoginPageParser { get; }
        INavigationBarParser NavigationBarParser { get; }
        IQueueBuildingParser QueueBuildingParser { get; }
        IQueueBuildingRepository QueueBuildingRepository { get; }
        IStockBarParser StockBarParser { get; }
        IStorageRepository StorageRepository { get; }
        ITroopPageParser TroopPageParser { get; }
        IVillagePanelParser VillagePanelParser { get; }
        IVillageRepository VillageRepository { get; }
        IVillageSettingRepository VillageSettingRepository { get; }
    }
}