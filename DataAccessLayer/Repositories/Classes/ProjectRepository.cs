
namespace DataAccessLayer.Repositories.Classes
{
    public class ProjectRepository(PortfolioDbContext _dbContext): GenericRepository<Project,int>(_dbContext), IProjectRepository
    {
    }
}
