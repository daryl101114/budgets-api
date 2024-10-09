using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class UserCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
