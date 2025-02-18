using System.ComponentModel.DataAnnotations;

namespace Presence.Models
{
    public class Organisation
    {
        [Key]
        public int Id { get; set; }

        public string ?Nom_Organisation { get; set; }

       

        public ICollection<Departement>?Departements { get; set; }
    }
}
