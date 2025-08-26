
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : IBaseEntity<TKey>
    {
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        Task<TEntity?> GetByIDAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity,bool>> predicate);
    }
}
