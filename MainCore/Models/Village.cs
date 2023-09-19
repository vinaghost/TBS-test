using System.ComponentModel.DataAnnotations.Schema;

namespace MainCore.Models
{
    public class Village : IEquatable<Village>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int AccountId { get; set; }
        public ICollection<Building> Buildings { get; set; }
        public ICollection<QueueBuilding> QueueBuildings { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public Storage Storage { get; set; }

        public bool Equals(Village other)
        {
            if (other is null)
                return false;

            return Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as Village);

        public override int GetHashCode() => Id;
    }
}