using System.ComponentModel.DataAnnotations;

namespace Presence.Models
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }

        public string ?ShiftName { get; set; }
        public TimeSpan Earliest_In { get; set; }
        public TimeSpan In_Time { get; set; }
        public TimeSpan Out_Time { get; set; }
        public TimeSpan Latest_Out { get; set; }
        public ICollection<Present> ?Presences { get; set; }
    }
}

