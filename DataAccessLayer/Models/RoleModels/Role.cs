
namespace DataAccessLayer.Models.RoleModels
{
    public class Role : IdentityRole, IBaseEntity<string>
    {
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
