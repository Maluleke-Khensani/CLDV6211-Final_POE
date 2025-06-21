using System.ComponentModel.DataAnnotations;

namespace Event_Ease.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        public string Name { get; set; }
        public List<Event>? Events { get; set; }
    }
}
