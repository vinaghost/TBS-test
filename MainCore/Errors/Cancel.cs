using FluentResults;

namespace MainCore.Errors
{
    public class Cancel : Error
    {
        public Cancel() : base("Pause button is pressed")
        {
        }
    }
}