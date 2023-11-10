using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class UnitOfWork : IUnitOfWork
    {
        #region repository

        public IAccountInfoRepository AccountInfoRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IAccountSettingRepository AccountSettingRepository { get; }
        public IBuildingRepository BuildingRepository { get; }
        public IFarmRepository FarmListRepository { get; }
        public IHeroItemRepository HeroItemRepository { get; }
        public IJobRepository JobRepository { get; }
        public IQueueBuildingRepository QueueBuildingRepository { get; }
        public IStorageRepository StorageRepository { get; }
        public IVillageRepository VillageRepository { get; }
        public IVillageSettingRepository VillageSettingRepository { get; }

        #endregion repository

        public UnitOfWork(IAccountInfoRepository accountInfoRepository, IAccountRepository accountRepository, IAccountSettingRepository accountSettingRepository, IBuildingRepository buildingRepository, IFarmRepository farmListRepository, IHeroItemRepository heroItemRepository, IJobRepository jobRepository, IQueueBuildingRepository queueBuildingRepository, IStorageRepository storageRepository, IVillageSettingRepository villageSettingRepository, IVillageRepository villageRepository)
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