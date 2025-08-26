
namespace DataAccessLayer.Repositories.Classes
{
    public class EducationRepository(PortfolioDbContext _dbContext): GenericRepository<Education,int>(_dbContext), IEducationRepository
    {
    }
}
