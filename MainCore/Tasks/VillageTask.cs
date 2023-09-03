namespace MainCore.Tasks
{
    public abstract class VillageTask : TaskBase
    {
        public int AccountId { get; private set; }
        public int VillageId { get; private set; }

        public void Setup(int accountId, int villageId, CancellationToken cancellationToken = default)
        {
            AccountId = accountId;
            VillageId = villageId;
            CancellationToken = cancellationToken;
        }
    }
}