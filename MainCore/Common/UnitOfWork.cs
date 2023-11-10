using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Parsers;
using MainCore.Repositories;

namespace MainCore.Common
{
    [RegisterAsTransient]
    public class UnitOfWork : IUnitOfWork
    {
        #region repositories

        public IAccountInfoRepository AccountInfoRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IAccountSettingRepository AccountSettingRepository { get; }
        public IBuildingRepository BuildingRepository { get; }
        public IFarmRepository FarmRepository { get; }
        public IHeroItemRepository HeroItemRepository { get; }
        public IJobRepository JobRepository { get; }
        public IQueueBuildingRepository QueueBuildingRepository { get; }
        public IStorageRepository StorageRepository { get; }
        public IVillageRepository VillageRepository { get; }
        public IVillageSettingRepository VillageSettingRepository { get; }

        #endregion repositories

        #region parsers

        public IAccountInfoParser AccountInfoParser { get; }
        public IBuildingParser BuildingParser { get; }
        public ICompleteImmediatelyParser CompleteImmediatelyParser { get; }
        public IFarmParser FarmParser { get; }
        public IFieldParser FieldParser { get; }
        public IHeroParser HeroParser { get; }
        public IInfrastructureParser InfrastructureParser { get; }
        public ILoginPageParser LoginPageParser { get; }
        public INavigationBarParser NavigationBarParser { get; }
        public IQueueBuildingParser QueueBuildingParser { get; }
        public IStockBarParser StockBarParser { get; }
        public ITroopPageParser TroopPageParser { get; }
        public IVillagePanelParser VillagePanelParser { get; }

        #endregion parsers

        public UnitOfWork(IAccountInfoRepository accountInfoRepository, IAccountRepository accountRepository, IAccountSettingRepository accountSettingRepository, IBuildingRepository buildingRepository, IFarmRepository farmListRepository, IHeroItemRepository heroItemRepository, IJobRepository jobRepository, IQueueBuildingRepository queueBuildingRepository, IStorageRepository storageRepository, IVillageSettingRepository villageSettingRepository, IVillageRepository villageRepository, IAccountInfoParser accountInfoParser, IBuildingParser buildingParser, ICompleteImmediatelyParser completeImmediatelyParser, IFarmParser farmParser, IFieldParser fieldParser, IHeroParser heroParser, IInfrastructureParser infrastructureParser, ILoginPageParser loginPageParser, INavigationBarParser navigationBarParser, IQueueBuildingParser queueBuildingParser, IStockBarParser stockBarParser, ITroopPageParser troopPageParser, IVillagePanelParser villagePanelParser)
        {
            AccountInfoRepository = accountInfoRepository;
            AccountRepository = accountRepository;
            AccountSettingRepository = accountSettingRepository;
            BuildingRepository = buildingRepository;
            FarmRepository = farmListRepository;
            HeroItemRepository = heroItemRepository;
            JobRepository = jobRepository;
            QueueBuildingRepository = queueBuildingRepository;
            StorageRepository = storageRepository;
            VillageSettingRepository = villageSettingRepository;
            VillageRepository = villageRepository;
            AccountInfoParser = accountInfoParser;
            BuildingParser = buildingParser;
            CompleteImmediatelyParser = completeImmediatelyParser;
            FarmParser = farmParser;
            FieldParser = fieldParser;
            HeroParser = heroParser;
            InfrastructureParser = infrastructureParser;
            LoginPageParser = loginPageParser;
            NavigationBarParser = navigationBarParser;
            QueueBuildingParser = queueBuildingParser;
            StockBarParser = stockBarParser;
            TroopPageParser = troopPageParser;
            VillagePanelParser = villagePanelParser;
        }
    }
}