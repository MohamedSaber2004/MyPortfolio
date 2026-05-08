using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class RoleChangeRequestConfigurations : IEntityTypeConfiguration<RoleChangeRequest>
    {
        public void Configure(EntityTypeBuilder<RoleChangeRequest> builder)
        {
            // Primary Key
            builder.HasKey(r => r.Id);

            // Foreign Key for requesting user
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign Key for admin who processed the request
            builder.HasOne(r => r.ProcessedByUser)
                .WithMany()
                .HasForeignKey(r => r.ProcessedById)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Properties configuration
            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.RejectionReason)
                .HasMaxLength(500);

            builder.Property(r => r.AdminNotes)
                .HasMaxLength(500);

            // Indexes for performance
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.CreatedOn);
        }
    }
}
