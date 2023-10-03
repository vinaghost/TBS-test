namespace MainCore.Common.Tasks
{
    public abstract class AccountTask : TaskBase
    {
        public int AccountId { get; private set; }

        public void Setup(int accountId, CancellationToken cancellationToken = default)
        {
            AccountId = accountId;
            CancellationToken = cancellationToken;
        }
    }
}