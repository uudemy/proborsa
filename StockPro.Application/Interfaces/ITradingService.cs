using StockPro.Domain.Entities;
using StockPro.Domain.Enums;

namespace StockPro.Application.Interfaces;

public interface ITradingService
{
    Task<Order> PlaceOrderAsync(
        Guid portfolioId,
        Guid symbolId,
        OrderSide side,
        OrderType type,
        decimal quantity,
        decimal? limitPrice,
        decimal? stopPrice,
        decimal? stopLossPrice,
        decimal? takeProfitPrice,
        CancellationToken cancellationToken);

    Task CancelOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<Order?> GetOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> GetOrdersAsync(
        Guid portfolioId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Trade>> GetTradesAsync(
        Guid portfolioId,
        CancellationToken cancellationToken);

    Task<Portfolio?> GetPortfolioAsync(
        Guid portfolioId,
        CancellationToken cancellationToken);
}