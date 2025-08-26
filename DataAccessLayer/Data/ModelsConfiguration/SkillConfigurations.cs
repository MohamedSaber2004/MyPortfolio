
namespace DataAccessLayer.Data.ModelsConfiguration
{
    public class SkillConfigurations : BaseEntityConfigurations<Skill>
    {
        public override void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.Property(s => s.SkillName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.ProficiencyLevel)
                   .IsRequired()
                   .HasConversion<string>();

            builder.HasOne(s => s.User)
                   .WithMany(u => u.Skills)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
