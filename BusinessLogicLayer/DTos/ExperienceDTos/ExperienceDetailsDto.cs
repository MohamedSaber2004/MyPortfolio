using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTos.ExperienceDTos
{
    public class ExperienceDetailsDto : IValidatableObject
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(150, ErrorMessage = "Company name cannot exceed {1} characters.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = null!;

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(150, ErrorMessage = "Role cannot exceed {1} characters.")]
        public string Role { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateOnly EndDate { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed {1} characters.")]
        public string? Description { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; } = null!;

        [Required]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [StringLength(150)]
        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "End Date cannot be earlier than Start Date.",
                    new[] { nameof(EndDate), nameof(StartDate) });
            }
        }
    }
}
