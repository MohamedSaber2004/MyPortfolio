using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTos.EducationDTos
{
    public class CreateOrUpdateEducationDto : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Institution name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Institution name must be between 2 and 200 characters.")]
        public string InstitutionName { get; set; } = null!;

        [Required(ErrorMessage = "Degree is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Degree must be between 2 and 150 characters.")]
        public string Degree { get; set; } = null!;

        [Required(ErrorMessage = "Field of study is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Field of study must be between 2 and 150 characters.")]
        public string FieldOfStudy { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }

        [StringLength(2000, ErrorMessage = "Description must be 2000 characters or fewer.")]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "End date must be on or after the start date.",
                    new[] { nameof(EndDate) });
            }
        }
    }
}
