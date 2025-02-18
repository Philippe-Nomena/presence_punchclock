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

        public string ?Nom { get; set; }
        public string ?Prenom { get; set; }
        public string ?Sexe { get; set; }
        public DateOnly Naissance { get; set; }
        public string ?Courriel { get; set; }
        public string ?Adresse { get; set; }
        public string ?Telephone { get; set; }
        public string ?Telephone_Urgence { get; set; }
        public string ?Barcode { get; set; }

        public Postal ?Postal { get; set; }
        public ICollection<Present> Presences { get; set; }
    }
}
