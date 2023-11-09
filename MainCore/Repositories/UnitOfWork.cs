using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Repositories
{
    [RegisterAsSingleton]
    public class UnitOfWork : IUnitOfWork
    {
        public IAccountInfoRepository AccountInfoRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IAccountSettingRepository AccountSettingRepository { get; }
        public IBuildingRepository BuildingRepository { get; }
        public IFarmListRepository FarmListRepository { get; }
        public IHeroItemRepository HeroItemRepository { get; }
        public IJobRepository JobRepository { get; }
        public IQueueBuildingRepository QueueBuildingRepository { get; }
        public IStorageRepository StorageRepository { get; }
        public IVillageRepository VillageRepository { get; }
        public IVillageSettingRepository VillageSettingRepository { get; }

        public UnitOfWork(IAccountInfoRepository accountInfoRepository, IAccountRepository accountRepository, IAccountSettingRepository accountSettingRepository, IBuildingRepository buildingRepository, IFarmListRepository farmListRepository, IHeroItemRepository heroItemRepository, IJobRepository jobRepository, IQueueBuildingRepository queueBuildingRepository, IStorageRepository storageRepository, IVillageSettingRepository villageSettingRepository, IVillageRepository villageRepository)
        {
            AccountInfoRepository = accountInfoRepository;
            AccountRepository = accountRepository;
            AccountSettingRepository = accountSettingRepository;
            BuildingRepository = buildingRepository;
            FarmListRepository = farmListRepository;
            HeroItemRepository = heroItemRepository;
            JobRepository = jobRepository;
            QueueBuildingRepository = queueBuildingRepository;
            StorageRepository = storageRepository;
            VillageSettingRepository = villageSettingRepository;
            VillageRepository = villageRepository;
        }
    }
}