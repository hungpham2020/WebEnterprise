using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models
{
    public class CustomUser : IdentityUser
    {
        public string? FullName { get; set; }

        public int? DepartmentId { get; set; }
    }
}
