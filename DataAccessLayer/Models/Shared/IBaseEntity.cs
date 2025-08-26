
namespace DataAccessLayer.Models.Shared
{
    public interface IBaseEntity<T>
    {
        T Id { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        string? LastModifiedBy { get; set; }
        DateTime? LastModifiedOn { get; set; }
        bool IsDeleted { get; set; }
    }
}
