using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Presence.Models
{
    public class Present
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Departement")]
        public int IdDepartement { get; set; }

        [ForeignKey("Employe")]
        public int IdEmploye { get; set; }

        [ForeignKey("Shift")]
        public int IdShift { get; set; }

        public DateOnly? Jour { get; set; }
        public bool Presente { get; set; }
        public string? JourIn { get; set; }
        public string?JourOut {  get; set; }
       
        public Departement ? Departement { get; set; }
        public Employe? Employe { get; set; }

        public Shift? Shift { get; set; }
    }
}
