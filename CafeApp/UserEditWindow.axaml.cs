using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CafeApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class UserEditWindow : Window
{
    private TextBox loginTBox;
    private TextBox passwordTBox;
    private ComboBox roleCBox;
    private User editUser;
    
    public List<Role> Roles { get; set; }
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public UserEditWindow()
    {
        InitializeComponent();
    }

    public UserEditWindow(User editUser)
    {
        InitializeComponent();
        
        loginTBox = this.FindControl<TextBox>("LoginTBox");
        passwordTBox = this.FindControl<TextBox>("PasswordTBox");
        roleCBox = this.FindControl<ComboBox>("RoleCBox");
        this.editUser = editUser;
        
        roleCBox.ItemsSource = _db.Roles.ToList();;

        loginTBox.Text = editUser.Login;
        passwordTBox.Text = editUser.Password;
        roleCBox.SelectedItem = editUser.Role;
    }
    
    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(loginTBox.Text) &&
            !string.IsNullOrWhiteSpace(passwordTBox.Text))
        {
            editUser.Login = loginTBox.Text;
            editUser.Password = passwordTBox.Text;
            editUser.Role = roleCBox.SelectedItem as Role;

            _db.SaveChanges();
            Close();
        }
    }

    private async void UserPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Открыть файл", AllowMultiple = false, FileTypeFilter = [FilePickerFileTypes.ImageAll] });
    }

    private async void ContractPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Открыть файл", AllowMultiple = false, FileTypeFilter = [FilePickerFileTypes.ImageAll] });
    }
}