using System.ComponentModel.DataAnnotations;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Models.Validation
{
    public class Date : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var model = (PostDTO)context.ObjectInstance;
            DateTime? openDate = Convert.ToDateTime(model?.OpenDate);
            DateTime? closedDate = Convert.ToDateTime(value);
            if(openDate != null && closedDate != null)
            {
                if (openDate >= closedDate)
                {
                    return new ValidationResult("Closed Date cannot greater than Open Date");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            return ValidationResult.Success;
        }
    }
}
