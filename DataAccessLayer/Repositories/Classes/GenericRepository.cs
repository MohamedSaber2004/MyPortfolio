

using System.Linq.Expressions;

namespace DataAccessLayer.Repositories.Classes
{
    public class GenericRepository<TEntity, TKey>(PortfolioDbContext _dbContext) : IGenericRepository<TEntity, TKey> where TEntity : class, IBaseEntity<TKey>
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false) =>
            withTracking == true ?
                await _dbContext.Set<TEntity>().Where(e => e.IsDeleted != true).ToListAsync() :
                await _dbContext.Set<TEntity>().Where(e => e.IsDeleted != true).AsNoTracking().ToListAsync();

        public async Task<TEntity?> GetByIDAsync(TKey id) =>
            await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Id!.Equals(id));

        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);

        public void Remove(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate) => await _dbContext.Set<TEntity>()
                                                                                                                          .Where(predicate)
                                                                                                                          .ToListAsync();
    }
}
