using System.ComponentModel.DataAnnotations;
using WebEnterprise.Models.Validation;

namespace WebEnterprise.Models.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:20, MinimumLength =10, ErrorMessage = "Title must have 10 to 20 digits")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OpenDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Date]
        public DateTime ClosedDate { get; set; }

        public int CatId { get; set; }

        public string? CatName { get; set; }

        public string UserId { get; set; }

        public string? AuthorName { get; set; }

        public IFormFile? FileUpload { get; set; }

        public string? FileName { get; set; }
    }
}
