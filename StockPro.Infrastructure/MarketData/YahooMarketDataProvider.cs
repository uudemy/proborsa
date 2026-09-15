using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using StockPro.Domain.Entities;
using StockPro.Domain.Enums;
using StockPro.Domain.Interfaces;

namespace StockPro.Infrastructure.MarketData;

public sealed class YahooMarketDataProvider : IMarketDataProvider
{
    private const string BaseUrl =
        "https://query1.finance.yahoo.com/v8/finance/chart/";

    private readonly HttpClient _httpClient;

    public YahooMarketDataProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Quote?> GetQuoteAsync(
        string symbol,
        CancellationToken cancellationToken)
    {
        var yahooSymbol = NormalizeYahooSymbol(symbol);

        var url =
            $"{BaseUrl}{Uri.EscapeDataString(yahooSymbol)}" +
            "?range=1d&interval=1d&includePrePost=false";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/140.0 Safari/537.36");

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken);

        using var document =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

        if (!document.RootElement
                .TryGetProperty("chart", out var chart))
        {
            return null;
        }

        if (!chart.TryGetProperty(
                "result",
                out var results))
        {
            return null;
        }

        if (results.ValueKind != JsonValueKind.Array ||
            results.GetArrayLength() == 0)
        {
            return null;
        }

        var result = results[0];

        if (!result.TryGetProperty(
                "meta",
                out var meta))
        {
            return null;
        }

        var lastPrice =
            GetDecimal(meta, "regularMarketPrice");

        if (lastPrice is null)
        {
            lastPrice =
                GetLastClose(result);
        }

        if (lastPrice is null)
        {
            return null;
        }

        var previousClose =
            GetDecimal(meta, "chartPreviousClose");

        if (previousClose is null)
        {
            previousClose =
                GetDecimal(meta, "previousClose");
        }

        if (previousClose is null)
        {
            previousClose = lastPrice;
        }

        var change =
            lastPrice.Value - previousClose.Value;

        var changePercent =
            previousClose.Value == 0
                ? 0
                : change / previousClose.Value * 100m;

        var open =
            GetLastValue(
                result,
                "open") ?? previousClose.Value;

        var high =
            GetLastValue(
                result,
                "high") ?? lastPrice.Value;

        var low =
            GetLastValue(
                result,
                "low") ?? lastPrice.Value;

        var volume =
            GetLastLongValue(
                result,
                "volume");

        var timestamp =
            GetMarketTimestamp(result);

        return new Quote
        {
            Id = Guid.NewGuid(),
            SymbolId = Guid.NewGuid(),
            LastPrice = Math.Round(
                lastPrice.Value,
                6),
            OpenPrice = Math.Round(
                open,
                6),
            HighPrice = Math.Round(
                Math.Max(high, lastPrice.Value),
                6),
            LowPrice = Math.Round(
                Math.Min(low, lastPrice.Value),
                6),
            PreviousClose = Math.Round(
                previousClose.Value,
                6),
            Change = Math.Round(
                change,
                6),
            ChangePercent = Math.Round(
                changePercent,
                4),
            Volume = volume,
            Timestamp = timestamp
        };
    }

    public async Task<IReadOnlyList<Candle>> GetHistoricalDataAsync(
        string symbol,
        TimeFrame timeframe,
        CancellationToken cancellationToken)
    {
        var yahooSymbol = NormalizeYahooSymbol(symbol);

        var settings = GetHistorySettings(timeframe);

        var url =
            $"{BaseUrl}{Uri.EscapeDataString(yahooSymbol)}" +
            $"?range={settings.Range}" +
            $"&interval={settings.Interval}" +
            "&includePrePost=false" +
            "&events=div%2Csplits";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/140.0 Safari/537.36");

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        await using var stream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken);

        using var document =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

        if (!document.RootElement
                .TryGetProperty("chart", out var chart))
        {
            return [];
        }

        if (!chart.TryGetProperty(
                "result",
                out var results))
        {
            return [];
        }

        if (results.ValueKind != JsonValueKind.Array ||
            results.GetArrayLength() == 0)
        {
            return [];
        }

        var result = results[0];

        if (!result.TryGetProperty(
                "timestamp",
                out var timestamps))
        {
            return [];
        }

        if (!result.TryGetProperty(
                "indicators",
                out var indicators))
        {
            return [];
        }

        if (!indicators.TryGetProperty(
                "quote",
                out var quotes) ||
            quotes.ValueKind != JsonValueKind.Array ||
            quotes.GetArrayLength() == 0)
        {
            return [];
        }

        var quote = quotes[0];

        if (!quote.TryGetProperty(
                "open",
                out var opens) ||
            !quote.TryGetProperty(
                "high",
                out var highs) ||
            !quote.TryGetProperty(
                "low",
                out var lows) ||
            !quote.TryGetProperty(
                "close",
                out var closes) ||
            !quote.TryGetProperty(
                "volume",
                out var volumes))
        {
            return [];
        }

        var candles = new List<Candle>();

        var count = timestamps.GetArrayLength();

        for (var i = 0; i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var timestamp =
                GetUnixDateTime(
                    timestamps[i]);

            var open =
                GetDecimalAtIndex(opens, i);

            var high =
                GetDecimalAtIndex(highs, i);

            var low =
                GetDecimalAtIndex(lows, i);

            var close =
                GetDecimalAtIndex(closes, i);

            var volume =
                GetLongAtIndex(volumes, i);

            if (open is null ||
                high is null ||
                low is null ||
                close is null)
            {
                continue;
            }

            candles.Add(
                new Candle
                {
                    Id = Guid.NewGuid(),
                    SymbolId = Guid.NewGuid(),
                    Timestamp = timestamp,
                    Open = open.Value,
                    High = high.Value,
                    Low = low.Value,
                    Close = close.Value,
                    Volume = volume
                });
        }

        return candles;
    }

    private static string NormalizeYahooSymbol(
        string symbol)
    {
        var normalized =
            symbol.Trim().ToUpperInvariant();

        return normalized switch
        {
            "BIST100" => "^XU100",
            "XU100" => "^XU100",

            "USDTRY" => "USDTRY=X",
            "USD/TRY" => "USDTRY=X",

            "EURTRY" => "EURTRY=X",
            "EUR/TRY" => "EURTRY=X",

            "GOLD" => "GC=F",
            "XAUUSD" => "GC=F",

            _ when normalized.EndsWith(
                ".IS",
                StringComparison.OrdinalIgnoreCase)
                => normalized,

            _ => $"{normalized}.IS"
        };
    }

    private static (
        string Range,
        string Interval) GetHistorySettings(
        TimeFrame timeframe)
    {
        return timeframe switch
        {
            TimeFrame.OneMinute =>
                ("1d", "1m"),

            TimeFrame.FiveMinutes =>
                ("5d", "5m"),

            TimeFrame.FifteenMinutes =>
                ("5d", "15m"),

            TimeFrame.ThirtyMinutes =>
                ("1mo", "30m"),

            TimeFrame.OneHour =>
                ("1mo", "1h"),

            TimeFrame.FourHours =>
                ("3mo", "1h"),

            TimeFrame.OneDay =>
                ("1y", "1d"),

            TimeFrame.OneWeek =>
                ("5y", "1wk"),

            TimeFrame.OneMonth =>
                ("10y", "1mo"),

            _ =>
                ("1mo", "1d")
        };
    }

    private static decimal? GetDecimal(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out var property))
        {
            return null;
        }

        return GetDecimalValue(property);
    }

    private static decimal? GetLastClose(
        JsonElement result)
    {
        return GetLastValue(
            result,
            "close");
    }

    private static decimal? GetLastValue(
        JsonElement result,
        string propertyName)
    {
        if (!result.TryGetProperty(
                "indicators",
                out var indicators))
        {
            return null;
        }

        if (!indicators.TryGetProperty(
                "quote",
                out var quotes) ||
            quotes.ValueKind != JsonValueKind.Array ||
            quotes.GetArrayLength() == 0)
        {
            return null;
        }

        var quote = quotes[0];

        if (!quote.TryGetProperty(
                propertyName,
                out var values))
        {
            return null;
        }

        for (var i = values.GetArrayLength() - 1;
             i >= 0;
             i--)
        {
            var value =
                GetDecimalValue(values[i]);

            if (value is not null)
            {
                return value;
            }
        }

        return null;
    }

    private static long GetLastLongValue(
        JsonElement result,
        string propertyName)
    {
        if (!result.TryGetProperty(
                "indicators",
                out var indicators))
        {
            return 0;
        }

        if (!indicators.TryGetProperty(
                "quote",
                out var quotes) ||
            quotes.ValueKind != JsonValueKind.Array ||
            quotes.GetArrayLength() == 0)
        {
            return 0;
        }

        var quote = quotes[0];

        if (!quote.TryGetProperty(
                propertyName,
                out var values))
        {
            return 0;
        }

        for (var i = values.GetArrayLength() - 1;
             i >= 0;
             i--)
        {
            var value =
                GetLongValue(values[i]);

            if (value is not null)
            {
                return value.Value;
            }
        }

        return 0;
    }

    private static decimal? GetDecimalAtIndex(
        JsonElement array,
        int index)
    {
        if (index >= array.GetArrayLength())
        {
            return null;
        }

        return GetDecimalValue(
            array[index]);
    }

    private static long GetLongAtIndex(
        JsonElement array,
        int index)
    {
        if (index >= array.GetArrayLength())
        {
            return 0;
        }

        return GetLongValue(
                   array[index])
               ?? 0;
    }

    private static decimal? GetDecimalValue(
        JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Number)
        {
            if (element.TryGetDecimal(
                    out var decimalValue))
            {
                return decimalValue;
            }

            if (element.TryGetDouble(
                    out var doubleValue))
            {
                return Convert.ToDecimal(
                    doubleValue,
                    CultureInfo.InvariantCulture);
            }
        }

        if (element.ValueKind == JsonValueKind.String &&
            decimal.TryParse(
                element.GetString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static long? GetLongValue(
        JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Number)
        {
            if (element.TryGetInt64(
                    out var integer))
            {
                return integer;
            }

            if (element.TryGetDouble(
                    out var doubleValue))
            {
                return Convert.ToInt64(
                    doubleValue,
                    CultureInfo.InvariantCulture);
            }
        }

        if (element.ValueKind == JsonValueKind.String &&
            long.TryParse(
                element.GetString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static DateTime GetMarketTimestamp(
        JsonElement result)
    {
        if (result.TryGetProperty(
                "meta",
                out var meta) &&
            meta.TryGetProperty(
                "regularMarketTime",
                out var marketTime))
        {
            var unix =
                GetLongValue(marketTime);

            if (unix is not null)
            {
                return DateTimeOffset
                    .FromUnixTimeSeconds(unix.Value)
                    .UtcDateTime;
            }
        }

        if (result.TryGetProperty(
                "timestamp",
                out var timestamps) &&
            timestamps.ValueKind == JsonValueKind.Array &&
            timestamps.GetArrayLength() > 0)
        {
            for (var i = timestamps.GetArrayLength() - 1;
                 i >= 0;
                 i--)
            {
                var unix =
                    GetLongValue(timestamps[i]);

                if (unix is not null)
                {
                    return DateTimeOffset
                        .FromUnixTimeSeconds(unix.Value)
                        .UtcDateTime;
                }
            }
        }

        return DateTime.UtcNow;
    }

    private static DateTime GetUnixDateTime(
        JsonElement element)
    {
        var unix =
            GetLongValue(element);

        if (unix is null)
        {
            return DateTime.UtcNow;
        }

        return DateTimeOffset
            .FromUnixTimeSeconds(unix.Value)
            .UtcDateTime;
    }
}