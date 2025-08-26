using DataAccessLayer.Models.Shared.Enums;

namespace BusinessLogicLayer.DTos.SkillDTos
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string SkillName { get; set; } = null!;
        public ProficiencyLevels ProficiencyLevel { get; set; }
        public bool IsDeleted { get; set; }
    }
}
