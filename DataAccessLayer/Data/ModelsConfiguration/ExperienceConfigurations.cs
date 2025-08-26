
namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class ExperienceConfigurations: BaseEntityConfigurations<Experience>
    {
        public new void Configure(EntityTypeBuilder<Experience> builder)
        {
            builder.Property(e => e.CompanyName)
           .IsRequired()
           .HasMaxLength(150);

            builder.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.StartDate)
                .IsRequired();

            builder.Property(e => e.EndDate)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(2000);

            builder.HasOne(e => e.User)
                .WithMany(u => u.Experiences)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
