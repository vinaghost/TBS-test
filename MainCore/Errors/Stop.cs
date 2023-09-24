using FluentResults;

namespace MainCore.Errors
{
    public class Stop : Error
    {
        public Stop(string message) : base($"{message}. Bot must stop")
        {
        }

        public static Stop EnglishRequired(string strType) => new($"Cannot parse {strType}. Is language English ?");
    }
}