
namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class BaseEntityConfigurations<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<int>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.CreatedOn).HasDefaultValueSql("GETDATE()");
            entityTypeBuilder.Property(b => b.LastModifiedOn).HasComputedColumnSql("GETDATE()");
        }
    }
}
