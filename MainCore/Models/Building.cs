using MainCore.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainCore.Models
{
    public class Building
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public bool IsUnderConstruction { get; set; }
        public int VillageId { get; set; }
    }
}