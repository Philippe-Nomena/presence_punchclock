using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Presence.Models
{
    public class Employe
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Postal")]
        public int IdCode_Postal { get; set; }
        [ForeignKey("Departement")]
        public int IdDepartement { get; set; }
        public string ? FirstName { get; set; }
        public string ? LastName { get; set; }
        public string ?Sexe { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateOnly HireDate { get; set; }
        public string ? Email { get; set; }
        public string ? Address { get; set; }
        public string ? City { get; set; }
        public string ? Region { get; set; }
        public string ? Country { get; set; }
        public string ? Phone { get; set; }
        public string ? Extension { get; set; }
        public string ? Photo { get; set; }
        public string ? EmployeeNotes    { get; set; }
        public string ?Barcode { get; set; }

        public Postal ?Postal { get; set; }
        public Departement ? Departement { get; set; }
        public ICollection<Present> Presences { get; set; }
    }
}
