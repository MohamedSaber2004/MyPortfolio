using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace BusinessLogicLayer.DTos.ProjectDTos
{
    public class CreatedOrUpdatedProjectDto : IValidatableObject
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
        public string ProjectURL { get; set; } = null!;

        [DataType(DataType.Upload)]
        public IFormFile? ProjectImage { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "End date must be on or after the start date.",
                    new[] { nameof(EndDate) });
            }

            if (ProjectImage is { Length: > 0 })
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var ext = Path.GetExtension(ProjectImage.FileName)?.ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(ext) || !allowed.Contains(ext))
                {
                    yield return new ValidationResult(
                        "Project image must be a JPG, JPEG, PNG, WEBP, or GIF file.",
                        new[] { nameof(ProjectImage) });
                }

                const long maxBytes = 2 * 1024 * 1024;
                if (ProjectImage.Length > maxBytes)
                {
                    yield return new ValidationResult(
                        "Project image must be 5 MB or smaller.",
                        new[] { nameof(ProjectImage) });
                }
            }
        }
    }
}
