using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Classes
{
    public class UnitOfWork(PortfolioDbContext _dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repositories = [];

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class, IBaseEntity<TKey>
        {
            var entityTypeName = typeof(TEntity).Name;

            if (_repositories.ContainsKey(entityTypeName)) return (IGenericRepository<TEntity, TKey>) _repositories[entityTypeName];
            else
            {
                var repository = new GenericRepository<TEntity, TKey>(_dbContext);

                _repositories[entityTypeName] = repository;

                return repository;
            }
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
