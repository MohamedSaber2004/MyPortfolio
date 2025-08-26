
namespace DataAccessLayer.Repositories.Classes
{
    public class SkillRepository(PortfolioDbContext _dbContext): GenericRepository<Skill,int>(_dbContext), ISkillRepository
    {
    }
}
