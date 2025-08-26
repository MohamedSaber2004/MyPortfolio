

namespace DataAccessLayer.Models.UserModels
{
    public class User: IdentityUser<string>, IBaseEntity<string>
    {
        public string FullName { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<SocialLink> SocialLinks { get; set; } = new HashSet<SocialLink>();

        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public ICollection<Skill> Skills { get; set; } = new HashSet<Skill>();

        public ICollection<Experience> Experiences { get; set; } = new HashSet<Experience>();

        public ICollection<Education> Educations { get; set; } = new HashSet<Education>();

        public ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
    }
}
