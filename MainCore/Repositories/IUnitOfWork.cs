namespace MainCore.Repositories
{
    public interface IUnitOfWork
    {
        IAccountInfoRepository AccountInfoRepository { get; }
        IAccountRepository AccountRepository { get; }
        IAccountSettingRepository AccountSettingRepository { get; }
        IBuildingRepository BuildingRepository { get; }
        IFarmRepository FarmListRepository { get; }
        IHeroItemRepository HeroItemRepository { get; }
        IJobRepository JobRepository { get; }
        IQueueBuildingRepository QueueBuildingRepository { get; }
        IStorageRepository StorageRepository { get; }
        IVillageRepository VillageRepository { get; }
        IVillageSettingRepository VillageSettingRepository { get; }
    }
}