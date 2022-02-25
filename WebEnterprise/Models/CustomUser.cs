using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models
{
    public class CustomUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
    }
}
