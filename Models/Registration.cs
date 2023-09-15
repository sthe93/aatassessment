// Registration.cs
using System.ComponentModel.DataAnnotations;

namespace EventRegistrationApp.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }
        public string ReferenceNumber { get; set; }

        public Event Event { get; set; } // Navigation property
    }
}
