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
    private readonly TextBox _loginTBox;
    private readonly TextBox _passwordTBox;
    private readonly ComboBox _roleCBox;
    
    private readonly User _editUser = new();
    
    public List<Role> Roles { get; set; }
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public UserEditWindow()
    {
        InitializeComponent();
        
        _loginTBox = this.FindControl<TextBox>("LoginTBox")!;
        _passwordTBox = this.FindControl<TextBox>("PasswordTBox")!;
        _roleCBox = this.FindControl<ComboBox>("RoleCBox")!;
        
        _roleCBox.ItemsSource = _db.Roles.ToList();;
    }

    public UserEditWindow(User editUser)
    {
        InitializeComponent();
        
        _loginTBox = this.FindControl<TextBox>("LoginTBox")!;
        _passwordTBox = this.FindControl<TextBox>("PasswordTBox")!;
        _roleCBox = this.FindControl<ComboBox>("RoleCBox")!;
        _editUser = editUser;
        
        _roleCBox.ItemsSource = _db.Roles.ToList();;

        _loginTBox.Text = editUser.Login;
        _passwordTBox.Text = editUser.Password;
        _roleCBox.SelectedItem = editUser.Role;
    }
    
    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_loginTBox.Text) &&
            !string.IsNullOrWhiteSpace(_passwordTBox.Text) &&
            _roleCBox.SelectedItem is Role selectedRole)
        {
            _editUser.Login = _loginTBox.Text;
            _editUser.Password = _passwordTBox.Text;
            _editUser.Role = selectedRole;
            _editUser.Status = UserStatusesConstants.USER_WORKED;
            
            if (_editUser.Id != 0)
                _db.Users.Update(_editUser);
            else
                await _db.Users.AddAsync(_editUser);
        
            await _db.SaveChangesAsync();
            
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