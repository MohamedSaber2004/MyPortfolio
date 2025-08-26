
namespace DataAccessLayer.Repositories.Classes
{
    public class RoleRepository(PortfolioDbContext _dbContext):GenericRepository<Role,string>(_dbContext), IRoleRepository
    {
    }
}
