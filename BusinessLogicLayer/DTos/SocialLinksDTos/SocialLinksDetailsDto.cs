using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTos.SocialLinksDTos
{
    public class SocialLinksDetailsDto
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

        [Required]
        [StringLength(150)]
        public string CreatedBy { get; set; } = null!;

        [StringLength(150)]
        public string? LastModifiedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
