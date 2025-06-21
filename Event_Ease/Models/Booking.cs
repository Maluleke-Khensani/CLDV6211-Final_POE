using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Event_Ease.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [ForeignKey("Venue")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

       

        [Required]
        public DateTime BookingDate { get; set; }

        
    }
}
