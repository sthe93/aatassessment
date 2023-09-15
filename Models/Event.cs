using System.ComponentModel.DataAnnotations;

namespace EventRegistrationApp.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Total seats is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Total seats must be greater than 0.")]
        public int TotalSeats { get; set; }
    }
}
