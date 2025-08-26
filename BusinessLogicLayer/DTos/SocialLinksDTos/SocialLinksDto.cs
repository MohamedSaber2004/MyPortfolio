using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTos.SocialLinksDTos
{
    public class SocialLinksDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Platform Name")]
        public string PlatformName { get; set; } = null!;

        [Required]
        [StringLength(500)]
        [DataType(DataType.Url)]
        public string URL { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
