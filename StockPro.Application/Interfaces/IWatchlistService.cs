using StockPro.Domain.Entities;

namespace StockPro.Application.Interfaces;

public interface IWatchlistService
{
    Task<IReadOnlyList<Watchlist>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Watchlist?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Watchlist> CreateAsync(
        string name,
        CancellationToken cancellationToken);

    Task RenameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task AddSymbolAsync(
        Guid watchlistId,
        Guid symbolId,
        CancellationToken cancellationToken);

    Task RemoveSymbolAsync(
        Guid watchlistId,
        Guid symbolId,
        CancellationToken cancellationToken);
}