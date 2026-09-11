using StockPro.Domain.Entities;

namespace StockPro.Application.Interfaces;

public interface IPortfolioService
{
    Task<Portfolio?> GetByIdAsync(
        Guid portfolioId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Position>> GetPositionsAsync(
        Guid portfolioId,
        CancellationToken cancellationToken);

    Task<Portfolio> CreateAsync(
        string name,
        decimal initialCash,
        CancellationToken cancellationToken);

    Task RefreshAsync(
        Guid portfolioId,
        CancellationToken cancellationToken);
}