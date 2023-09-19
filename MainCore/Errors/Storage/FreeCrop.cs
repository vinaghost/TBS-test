using FluentResults;

namespace MainCore.Errors.Storage
{
    public class FreeCrop : Error
    {
        public FreeCrop(long storage, long required) : base($"Don't have enough freecrop [{storage} < {required}]")
        {
        }
    }
}