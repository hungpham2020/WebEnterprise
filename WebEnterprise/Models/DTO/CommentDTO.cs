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

        public string? PostAuthorName { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? OpenDate { get; set; }

        public string? CatName { get; set; }

        public string ToSeparatedString(string r)
        {
            return $"{this.Description}{r}" +
                    $"{this.PostId}{r}" +
                    $"{this.CommentId}";
        }
    }
}
