using FluentResults;

namespace MainCore.Errors.Storage
{
    public class WarehouseLimit : Error
    {
        public WarehouseLimit(long storage, long required) : base($"Don't have enough storage [{storage} < {required}]")
        {
        }
    }
}