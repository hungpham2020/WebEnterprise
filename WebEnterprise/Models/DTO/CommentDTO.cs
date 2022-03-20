using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models.DTO
{
    public class CommentDTO
    {
        public int? CommentId { get; set; }
        public string? AuthorName { get; set; }

        public int? PostId { get; set; }

        public string? Description { get; set; }
        
        public string? PostDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? UpdateTime{ get; set; }
    }
}
