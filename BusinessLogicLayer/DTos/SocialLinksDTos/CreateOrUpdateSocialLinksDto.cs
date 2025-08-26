using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTos.SocialLinksDTos
{
    public class CreateOrUpdateSocialLinksDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Platform name is required.")]
        [StringLength(150, ErrorMessage = "Platform name cannot exceed {1} characters.")]
        [Display(Name = "Platform Name")]
        public string PlatformName { get; set; } = null!;

        [Required(ErrorMessage = "URL is required.")]
        [StringLength(500, ErrorMessage = "URL cannot exceed {1} characters.")]
        [DataType(DataType.Url)]
        public string URL { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}
