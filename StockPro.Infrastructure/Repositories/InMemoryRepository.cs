namespace StockPro.Infrastructure.Repositories;

public sealed class InMemoryRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly List<TEntity> _entities = [];

    public Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = _entities.FirstOrDefault(entity =>
            entity.GetType()
                .GetProperty("Id")?
                .GetValue(entity) is Guid entityId &&
            entityId == id);

        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<TEntity> result = _entities.ToList();

        return Task.FromResult(result);
    }

    public Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        cancellationToken.ThrowIfCancellationRequested();

        _entities.Add(entity);

        return Task.CompletedTask;
    }

    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var idProperty = entity.GetType().GetProperty("Id");

        if (idProperty?.GetValue(entity) is not Guid id)
        {
            throw new InvalidOperationException(
                $"Entity type '{typeof(TEntity).Name}' must have a Guid Id property.");
        }

        var existingIndex = _entities.FindIndex(existing =>
            existing.GetType()
                .GetProperty("Id")?
                .GetValue(existing) is Guid existingId &&
            existingId == id);

        if (existingIndex < 0)
        {
            throw new KeyNotFoundException(
                $"Entity with id '{id}' was not found.");
        }

        _entities[existingIndex] = entity;
    }

    public void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _entities.Remove(entity);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.CompletedTask;
    }
}