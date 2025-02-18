using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Presence.Models
{
    public class Departement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Organisation")]
        public int IdOrganisation { get; set; }

        public string?NomDepartement { get; set; }

        public Organisation?Organisation { get; set; }

        public ICollection<Present>? Presences { get; set; }
    }
}
