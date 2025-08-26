

namespace DataAccessLayer.Models.Shared
{
    public abstract class BaseEntity<T> : IBaseEntity<T>
    {
        public required T Id { get; set; } 
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
