using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class MainWindow : Window
{
    private TextBox loginTBox;
    private TextBox passwordTBox;
    private TextBlock messageTBlock;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public MainWindow()
    {
        InitializeComponent();
        
        loginTBox = this.FindControl<TextBox>("LoginTBox");
        passwordTBox = this.FindControl<TextBox>("PasswordTBox");
        messageTBlock = this.FindControl<TextBlock>("MessageTBlock");
    }

    private void AuthBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(loginTBox.Text) || string.IsNullOrWhiteSpace(passwordTBox.Text))
        {
            messageTBlock.Text = "Поля логина или пароля пустые";
            messageTBlock.IsVisible = true;
            return;
        }
        
        var userAuth = _db.Users.Include(x => x.Role).FirstOrDefault(u => u.Login == loginTBox.Text && u.Password == passwordTBox.Text);
        var userRole = userAuth?.Role;

        if (userAuth == null)
        {
            messageTBlock.Text = "Введенные логин или пароль неверны";
            messageTBlock.IsVisible = true;
            return;
        }

        if (userRole?.Name == RolesConstants.ADMIN_ROLE)
        {
            new AdminWindow().Show();
            Close();
        }
        else if (userRole?.Name == RolesConstants.WAITER_ROLE)
        {
            new AdminWindow().Show();
            Close();
        }
        else if (userRole?.Name == RolesConstants.COOK_ROLE)
        {
            new AdminWindow().Show();
            Close();
        }
    }

    private void RegBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}