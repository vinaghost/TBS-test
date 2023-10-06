using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Navigate.Parsers.FieldParser
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : IFieldParser
    {
        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            return null;
        }
    }
}