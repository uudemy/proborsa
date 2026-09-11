using Microsoft.Extensions.DependencyInjection;

namespace StockPro.Application.Common;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddStockProApplication(
        this IServiceCollection services)
    {
        return services;
    }
}