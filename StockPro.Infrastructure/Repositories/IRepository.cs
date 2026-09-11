namespace StockPro.Infrastructure.Repositories;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken);

    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}