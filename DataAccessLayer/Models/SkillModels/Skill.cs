
namespace DataAccessLayer.Models.SkillModels
{
    public class Skill : BaseEntity<int>
    {
        public string SkillName { get; set; } = null!;
        public ProficiencyLevels ProficiencyLevel { get; set; }

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
