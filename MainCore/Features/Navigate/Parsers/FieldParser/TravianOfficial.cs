using HtmlAgilityPack;
using MainCore.Common.Enums;
using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Features.Navigate.Parsers.FieldParser
{
    [RegisterAsTransient(ServerEnums.TravianOfficial)]
    public class TravianOfficial : IFieldParser
    {
        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            return doc
                .DocumentNode
                .Descendants("a")
                .FirstOrDefault(x => x.HasClass($"buildingSlot{index}"));
        }
    }
}