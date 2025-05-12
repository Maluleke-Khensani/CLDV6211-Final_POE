using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Ease.Models
{
    public class Event
    {
        [Key]  // Marks this as the Primary Key
        public int EventId { get; set; }

        [Required]  // Ensures the field is not left empty
        public string EventName { get; set; }

        [Required]

        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }

        // Foreign Key (FK) to Venue
        //[Required]  // Ensures an event must be linked to a venue
        public int VenueId { get; set; }

        //[ForeignKey("VenueId")]  // Links this to the Venue model
        public Venue? Venue { get; set; } = null;
        public virtual ICollection<Booking> Booking { get; set; } = new List<Booking>(); 
    }
}
