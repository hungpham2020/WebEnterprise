﻿using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models
{
    public class Comment
    {
        public int Id { get; set; }


        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime UpdateTime { get; set; }

        public string? AuthorId { get; set; }

        public int? PostId { get; set; }
    }
}
