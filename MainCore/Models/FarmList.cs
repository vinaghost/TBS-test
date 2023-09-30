using System.ComponentModel.DataAnnotations.Schema;

namespace MainCore.Models
{
    public class FarmList
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int AccountId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}