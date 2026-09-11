using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StockPro.App.ViewModels;
using StockPro.Application.Common;
using StockPro.Infrastructure.DependencyInjection;

namespace StockPro.App;

public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddStockProApplication();
        services.AddStockProInfrastructure();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

        MainWindow = mainWindow;

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();

        base.OnExit(e);
    }
}