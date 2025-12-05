using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CafeApp.CookWindows;

public partial class CookWindow : Window
{
    public CookWindow()
    {
        InitializeComponent();
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}