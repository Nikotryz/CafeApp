using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class RegistrationWindow : Window
{
    private TextBox loginTBox;
    private TextBox passwordTBox;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public RegistrationWindow()
    {
        InitializeComponent();

        loginTBox = this.FindControl<TextBox>("LoginTBox")!;
        passwordTBox = this.FindControl<TextBox>("PasswordTBox")!;
    }

    private async void RegBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(loginTBox.Text) &&
            !string.IsNullOrWhiteSpace(passwordTBox.Text))
        {
            var newUser = new User
            {
                Login = loginTBox.Text,
                Password = passwordTBox.Text,
                Role = _db.Roles.FirstOrDefault(r => r.Name == RolesConstants.WAITER_ROLE),
                Status = UserStatusesConstants.USER_WORKED
            };
            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            new MainWindow().Show();
            Close();
        }
    }
    
    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}