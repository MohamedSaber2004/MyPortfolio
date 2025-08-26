using DataAccessLayer.Models.Shared.Enums;

namespace BusinessLogicLayer.DTos.SkillDTos
{
    public class SkillDetailsDto
    {
        public int Id { get; set; }
        public string SkillName { get; set; } = null!;
        public ProficiencyLevels ProficiencyLevel { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
