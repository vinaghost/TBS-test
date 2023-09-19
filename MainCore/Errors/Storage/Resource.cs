using FluentResults;

namespace MainCore.Errors.Storage
{
    public class Resource : Error
    {
        public Resource(string resource, long storage, long required) : base($"Don't have enough {resource} [{storage} < {required}]")
        {
        }
    }
}