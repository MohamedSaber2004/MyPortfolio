namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity,TKey> GetRepository<TEntity,TKey>() where TEntity: class, IBaseEntity<TKey>;

        Task<int> SaveChangesAsync();
    }
}
