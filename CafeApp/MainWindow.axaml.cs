using System;
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
            ShowMessage("Поля логина или пароля пустые");
            return;
        }
        
        var user = _db.Users
            .Include(x => x.Role)
            .FirstOrDefault(u => u.Login == _loginTBox.Text);
        var passwordIsValid = PasswordHasher.IsValid(_passwordTBox.Text, user?.PasswordHash ?? string.Empty);
        
        if (user == null || !passwordIsValid)
        {
            ShowMessage("Введенные логин или пароль неверны");
            return;
        }
        
        var shift = _db.Shifts
            .Include(x => x.Users)
            .Include(x => x.WaiterTables)
            .FirstOrDefault(x => 
                x.ShiftStarted <= DateTime.Now.ToLocalTime() && 
                x.ShiftEnds >= DateTime.Now.ToLocalTime() && 
                (x.Users.Contains(user) || user.Role.Name == Roles.ADMIN_ROLE));
        
        if (shift != null)
            App.CurrentShift = shift;
        App.CurrentUser = user;
        
        switch (user.Role.Name)
        {
            case Roles.ADMIN_ROLE:
                new AdminWindow().Show();
                break;
            case Roles.COOK_ROLE:
                if (shift == null)
                {
                    ShowMessage("У вас не назначена смена");
                    return;
                }
                new CookWindow().Show();
                break;
            case Roles.WAITER_ROLE:
                if (shift == null)
                {
                    ShowMessage("У вас не назначена смена");
                    return;
                }
                new WaiterWindow().Show();
                break;
        }
        
        Close();
    }

    private void ShowMessage(string message)
    {
        _messageTBlock.Text = message;
        _messageTBlock.IsVisible = true;
    }
}