using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BusinessLogicLayer.DTos.ContactDTos
{
    public class CreateOrUpdateContactDto : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Contact type is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Contact type must be between 2 and 50 characters.")]
        public string ContactType { get; set; } = null!;

        [Required(ErrorMessage = "Contact value is required.")]
        [StringLength(512, MinimumLength = 3, ErrorMessage = "Contact value must be between 3 and 512 characters.")]
        public string ContactValue { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var type = ContactType?.Trim();
            var value = ContactValue?.Trim();

            if (string.IsNullOrWhiteSpace(type))
            {
                yield return new ValidationResult("Contact type is required.", new[] { nameof(ContactType) });
                yield break;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                yield return new ValidationResult("Contact value is required.", new[] { nameof(ContactValue) });
                yield break;
            }

            switch (type.ToLowerInvariant())
            {
                case "email":
                    var emailAttr = new EmailAddressAttribute();
                    if (!emailAttr.IsValid(value))
                        yield return new ValidationResult("Contact value must be a valid email address.", new[] { nameof(ContactValue) });
                    break;

                case "phone":
                case "mobile":
                case "telephone":
                    var phoneRegex = new Regex(@"^\+?[0-9\s\-\(\)]{6,20}$");
                    if (!phoneRegex.IsMatch(value))
                        yield return new ValidationResult("Contact value must be a valid phone number.", new[] { nameof(ContactValue) });
                    break;

                case "website":
                case "url":
                case "linkedin":
                case "github":
                case "twitter":
                case "facebook":
                case "instagram":
                case "whatsapp":
                case "telegram":
                    var urlAttr = new UrlAttribute();
                    if (!urlAttr.IsValid(value))
                        yield return new ValidationResult("Contact value must be a valid URL.", new[] { nameof(ContactValue) });
                    break;

                default:
                    break;
            }
        }
    }
}
