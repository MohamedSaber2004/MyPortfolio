
namespace DataAccessLayer.Repositories.Classes
{
    public class ExperienceRepository(PortfolioDbContext _dbContext): GenericRepository<Experience,int>(_dbContext), IExperienceRepository
    {
    }
}
