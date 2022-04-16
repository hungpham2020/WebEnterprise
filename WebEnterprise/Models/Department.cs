using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 10, MinimumLength = 2, ErrorMessage = "Name must be 4 to 10 digits")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 4, ErrorMessage = "Description must be 4 to 30 digits")]
        public string Description { get; set; }
    }
}
