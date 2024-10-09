using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class AuthenticationRequestBodyDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
