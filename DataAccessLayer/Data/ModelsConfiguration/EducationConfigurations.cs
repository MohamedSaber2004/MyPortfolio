
namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class EducationConfigurations: BaseEntityConfigurations<Education>
    {
        public new void Configure(EntityTypeBuilder<Education> builder)
        {
            builder.Property(e => e.InstitutionName)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(e => e.Degree)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.FieldOfStudy)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.StartDate)
                   .IsRequired();

            builder.Property(e => e.EndDate)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(2000);

            builder.HasOne(e => e.User)
                   .WithMany(u => u.Educations)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
