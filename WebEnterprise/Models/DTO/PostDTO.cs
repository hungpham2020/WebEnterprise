﻿using System.ComponentModel.DataAnnotations;
using WebEnterprise.Models.Validation;

namespace WebEnterprise.Models.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:100, MinimumLength =5, ErrorMessage = "Title must have 5 to 100 digits")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? OpenDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ClosedDate { get; set; }
        public int? CatId { get; set; }

        public string? CatName { get; set; }

        public string? UserId { get; set; }

        public string? AuthorName { get; set; }

        public int? Like { get; set; }

        public int? DisLike { get; set; }

        public IFormFile? FileUpload { get; set; }

        public string? FileName { get; set; }

        public bool? Status { get; set; }
    }
}
