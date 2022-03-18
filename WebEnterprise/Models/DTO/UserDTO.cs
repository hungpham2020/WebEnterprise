using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }

        [Required]
        [StringLength(maximumLength: 10, MinimumLength = 5, ErrorMessage = "Username must have 5 to 10 digits")]
        public string UserName { get; set; }

        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 5, ErrorMessage = "FullName must have 5 to 20 digits")]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
