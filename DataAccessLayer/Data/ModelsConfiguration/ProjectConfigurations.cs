using DataAccessLayer.Models.ProjectModels;

namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class ProjectConfigurations : BaseEntityConfigurations<Project>
    {
        public new void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(150);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000); 

            builder.Property(p => p.StartDate)
                .IsRequired();

            builder.Property(p => p.EndDate)
                .IsRequired();

            builder.Property(p => p.ProjectURL)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false); 

            builder.Property(p => p.ProjectImage)
                .IsRequired(false)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Projects)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
