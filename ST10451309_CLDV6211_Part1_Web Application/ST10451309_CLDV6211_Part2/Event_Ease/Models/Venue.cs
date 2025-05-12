using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Ease.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string Location { get; set; }

        public int Capacity { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped] // This property will not be mapped to the database
        public  IFormFile ? ImageFile { get; set; } // This property will hold the uploaded image file
      
        //This property will hold a list of events that are associated with this venue
        public List<Event> Events { get; set; } = new();

    
    }
}
