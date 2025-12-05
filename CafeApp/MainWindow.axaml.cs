using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using CafeApp.AdminWindows;
using CafeApp.CookWindows;
using CafeApp.WaiterWindows;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class MainWindow : Window
{
    private readonly TextBox _loginTBox;
    private readonly TextBox _passwordTBox;
    private readonly TextBlock _messageTBlock;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public MainWindow()
    {
        InitializeComponent();
        
        _loginTBox = this.FindControl<TextBox>("LoginTBox")!;
        _passwordTBox = this.FindControl<TextBox>("PasswordTBox")!;
        _messageTBlock = this.FindControl<TextBlock>("MessageTBlock")!;
    }

    private void AuthBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_loginTBox.Text) || string.IsNullOrWhiteSpace(_passwordTBox.Text))
        {
            _messageTBlock.Text = "Поля логина или пароля пустые";
            _messageTBlock.IsVisible = true;
            return;
        }
        
        var userAuth = _db.Users
            .Include(x => x.Role)
            .FirstOrDefault(u => u.Login == _loginTBox.Text);
        var passwordIsValid = PasswordHasher.IsValid(_passwordTBox.Text, userAuth?.PasswordHash ?? string.Empty);
        
        if (userAuth == null || !passwordIsValid)
        {
            _messageTBlock.Text = "Введенные логин или пароль неверны";
            _messageTBlock.IsVisible = true;
            return;
        }
        
        var userRole = userAuth.Role;

        switch (userRole.Name)
        {
            case Roles.ADMIN_ROLE:
                new AdminWindow().Show();
                break;
            case Roles.COOK_ROLE:
                new CookWindow().Show();
                break;
            case Roles.WAITER_ROLE:
                new WaiterWindow().Show();
                break;
        }
        
        Close();
    }
}