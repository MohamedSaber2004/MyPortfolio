
namespace DataAccessLayer.Repositories.Classes
{
    public class UserRepository(PortfolioDbContext _dbContext): GenericRepository<User,string>(_dbContext), IUserRepository
    {

    }
}
