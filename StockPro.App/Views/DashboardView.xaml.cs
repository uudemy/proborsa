using System.Windows.Controls;
using StockPro.App.ViewModels;

namespace StockPro.App.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();

        DataContext = new DashboardViewModel();
    }
}