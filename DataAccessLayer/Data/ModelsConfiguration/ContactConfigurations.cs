
using DataAccessLayer.Models.ContactModels;

namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class ContactConfigurations: BaseEntityConfigurations<Contact>
    {
        public new void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(c => c.ContactType)
                  .IsRequired()
                  .HasMaxLength(50);

            builder.Property(c => c.ContactValue)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Contacts)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
