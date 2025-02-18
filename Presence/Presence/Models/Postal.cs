using System.ComponentModel.DataAnnotations;

namespace Presence.Models
{
    public class Postal
    {
        [Key]
        public int Id { get; set; }

        public string ?Code_Postal { get; set; }
        public string ?Pays { get; set; }

        public ICollection<Employe>? Employes { get; set; }
    }
}
