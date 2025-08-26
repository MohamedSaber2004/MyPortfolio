using DataAccessLayer.Models.SocialLinksModels;

namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class SocialConfigurations: BaseEntityConfigurations<SocialLink>
    {
        public new void Configure(EntityTypeBuilder<SocialLink> builder)
        {
            builder.Property(s => s.PlatformName)
            .IsRequired()
            .HasMaxLength(60);

            builder.Property(s => s.URL)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false); 

            base.Configure(builder);
        }
    }
}
