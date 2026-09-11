using StockPro.Domain.Models;

namespace StockPro.Domain.Interfaces;

public interface IStreamingMarketDataProvider
{
    Task SubscribeAsync(
        IEnumerable<string> symbols,
        CancellationToken cancellationToken);

    Task UnsubscribeAsync(
        IEnumerable<string> symbols,
        CancellationToken cancellationToken);

    event EventHandler<QuoteUpdatedEventArgs>? QuoteUpdated;
}