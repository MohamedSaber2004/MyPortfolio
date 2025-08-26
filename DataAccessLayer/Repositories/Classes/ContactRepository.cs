
namespace DataAccessLayer.Repositories.Classes
{
    public class ContactRepository(PortfolioDbContext _dbContext): GenericRepository<Contact,int>(_dbContext),IContactRepository
    {
    }
}
