using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CafeApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class UserEditWindow : Window
{
    private TextBox _loginTBox;
    private TextBox _passwordTBox;
    private TextBox _nameTBox;
    private TextBox _lastNameTBox;
    private TextBox _middleNameTBox;
    private DatePicker _birthdayDPicker;
    private ComboBox _roleCBox;
    private ComboBox _statusCBox;

    private Image _employeeImageBox;
    private Image _contractImageBox;
    
    private readonly User _editUser = new();
    
    public List<Role> Roles { get; set; }
    public List<string> Statuses { get; set; }
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public UserEditWindow()
    {
        InitializeComponent();
        
        FindControls();
        
        var defaultRole = _db.Roles.FirstOrDefault(x => x.Name == RolesConstants.WAITER_ROLE);
        
        _roleCBox!.ItemsSource = _db.Roles.ToList();
        _roleCBox.SelectedItem = defaultRole;
        
        _statusCBox!.ItemsSource = new string[] {UserStatusesConstants.USER_WORKED, UserStatusesConstants.USER_FIRED};
        _statusCBox.SelectedItem = UserStatusesConstants.USER_WORKED;
    }

    public UserEditWindow(User editUser)
    {
        InitializeComponent();
        
        FindControls();
        
        _editUser = editUser;
        
        _roleCBox!.ItemsSource = _db.Roles.ToList();
        _statusCBox!.ItemsSource = new string[] { UserStatusesConstants.USER_WORKED, UserStatusesConstants.USER_FIRED };

        _loginTBox!.Text = editUser.Login;
        _passwordTBox!.Text = editUser.Password;
        _roleCBox.SelectedItem = editUser.Role;
        _statusCBox!.SelectedItem = editUser.Status;
        _nameTBox!.Text = editUser.Name;
        _lastNameTBox!.Text = editUser.LastName;
        _middleNameTBox!.Text = editUser.MiddleName;
        _birthdayDPicker!.SelectedDate = editUser.Birthday.ToDateTime(new TimeOnly(0, 0));
        _employeeImageBox!.Source = editUser.UserPhoto?.ToBitmap();
        _contractImageBox!.Source = editUser.ContractPhoto?.ToBitmap();
    }

    private void FindControls()
    {
        _loginTBox = this.FindControl<TextBox>("LoginTBox")!;
        _passwordTBox = this.FindControl<TextBox>("PasswordTBox")!;
        _roleCBox = this.FindControl<ComboBox>("RoleCBox")!;
        _nameTBox = this.FindControl<TextBox>("NameTBox")!;
        _lastNameTBox = this.FindControl<TextBox>("LastNameTBox")!;
        _middleNameTBox = this.FindControl<TextBox>("MiddleNameTBox")!;
        _birthdayDPicker = this.FindControl<DatePicker>("BirthdayDPicker")!;
        _employeeImageBox = this.FindControl<Image>("EmployeeImageBox")!;
        _contractImageBox = this.FindControl<Image>("ContractImageBox")!;
        _statusCBox = this.FindControl<ComboBox>("StatusCBox")!;
    }
    
    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_loginTBox.Text) &&
            !string.IsNullOrWhiteSpace(_passwordTBox.Text) &&
            !string.IsNullOrWhiteSpace(_nameTBox.Text) &&
            !string.IsNullOrWhiteSpace(_lastNameTBox.Text) &&
            !string.IsNullOrWhiteSpace(_middleNameTBox.Text) &&
            _birthdayDPicker.SelectedDate != null &&
            _roleCBox.SelectedItem is Role selectedRole &&
            _statusCBox.SelectedItem is string status)
        {
            _editUser.Login = _loginTBox.Text;
            _editUser.Password = _passwordTBox.Text;
            _editUser.Role = selectedRole;
            _editUser.Name = _nameTBox.Text;
            _editUser.LastName = _lastNameTBox.Text;
            _editUser.MiddleName = _middleNameTBox.Text;
            _editUser.Birthday = DateOnly.FromDateTime(_birthdayDPicker.SelectedDate.Value.Date);
            _editUser.Status = status;
            
            if (_editUser.Id != 0)
                _db.Users.Update(_editUser);
            else
                _db.Users.Add(_editUser);
        
            _db.SaveChanges();
            
            Close();
        }
    }

    private async void UserPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await GetImageAsync();

        if (file == null)
            return;
        
        _editUser.UserPhoto = await file.ReadAllBytesAsync();
        _employeeImageBox.Source = _editUser.UserPhoto.ToBitmap();
    }

    private async void ContractPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await GetImageAsync();

        if (file == null)
            return;
        
        _editUser.ContractPhoto = await file.ReadAllBytesAsync();
        _contractImageBox.Source = _editUser.ContractPhoto.ToBitmap();
    }

    private async Task<IStorageFile?> GetImageAsync()
    {
        var topLevel = GetTopLevel(this);
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Выберите фото", AllowMultiple = false, FileTypeFilter = [FilePickerFileTypes.ImageAll] });

        return files.FirstOrDefault();
    }
}