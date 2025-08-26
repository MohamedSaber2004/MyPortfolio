using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Models.UserModels;

namespace BusinessLogicLayer.DTos.ProjectDTos
{
    public class ProjectDetailsDto : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 4000 characters.")]
        public string Description { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Project URL is required.")]
        [Url(ErrorMessage = "Project URL must be a valid URL.")]
        [StringLength(2048, ErrorMessage = "Project URL must be 2048 characters or fewer.")]
        public string ProjectURL { get; set; } = null!;

        [Required(ErrorMessage = "Project image is required.")]
        [FileExtensions(Extensions = "jpg,jpeg,png,webp,gif", ErrorMessage = "Project image must be a JPG, JPEG, PNG, WEBP, or GIF filename or path.")]
        [StringLength(512, ErrorMessage = "Project image path must be 512 characters or fewer.")]
        public string ProjectImage { get; set; } = null!;

        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "CreatedBy is required.")]
        [StringLength(256)]
        public string CreatedBy { get; set; } = null!;

        [StringLength(256)]
        public string? LastModifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedOn { get; set; }

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
