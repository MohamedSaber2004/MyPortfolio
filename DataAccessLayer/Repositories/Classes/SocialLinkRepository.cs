namespace DataAccessLayer.Repositories.Classes
{
    public class SocialLinkRepository(PortfolioDbContext _dbContext): GenericRepository<SocialLink,int>(_dbContext), ISocialLinkRepository
    {
    }
}
