using StockPro.Domain.Entities;

namespace StockPro.Application.Interfaces;

public interface IAlertService
{
    Task<IReadOnlyList<Alert>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Alert?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Alert> CreateAsync(
        Alert alert,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Alert alert,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Alert>> EvaluateAsync(
        IReadOnlyList<Quote> quotes,
        CancellationToken cancellationToken);
}