using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class ShiftEditWindow : Window
{
    private readonly Shift _editShift = new();
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();

    private readonly DatePicker _shiftDPicker;
    private readonly TimePicker _shiftStartTPicker;
    private readonly TimePicker _shiftEndTPicker;
    private readonly DataGrid _shiftUsersDataGrid;
    
    private readonly TextBlock _errorTextBlock;

    public List<SelectionUser> Users { get; set; } = [];
    
    public ShiftEditWindow()
    {
        InitializeComponent();
        
        _shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        _shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        _shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        _shiftUsersDataGrid = this.FindControl<DataGrid>("ShiftUsersDataGrid")!;
        
        _errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock")!;
        
        LoadUsers();
    }

    public ShiftEditWindow(Shift shift)
    {
        InitializeComponent();
        
        _shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        _shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        _shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        _shiftUsersDataGrid = this.FindControl<DataGrid>("ShiftUsersDataGrid")!;
        
        _errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock")!;
        
        _editShift = shift;

        var date = shift.ShiftStarted.Date;
        var startTime = shift.ShiftStarted.TimeOfDay;
        var endTime = shift.ShiftEnds.TimeOfDay;
        
        _shiftDPicker.SelectedDate = date;
        _shiftStartTPicker.SelectedTime = startTime;
        _shiftEndTPicker.SelectedTime = endTime;
        
        LoadUsers();
    }

    private void LoadUsers()
    {
        var allUsers = _db.Users
            .Include(x => x.Role)
            .Include(x => x.Shifts)
            .Where(x => x.Status == UserStatusesConstants.USER_WORKED)
            .ToList();
        
        var selectionUsers = new List<SelectionUser>();

        foreach (var user in allUsers)
            selectionUsers.Add(new SelectionUser { User = user, IsSelected = user.Shifts.Contains(_editShift) });
        
        Users = selectionUsers;
        _shiftUsersDataGrid.ItemsSource = Users;
    }

    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_shiftDPicker.SelectedDate == null ||
            _shiftStartTPicker.SelectedTime == null ||
            _shiftEndTPicker.SelectedTime == null)
        {
            _errorTextBlock.Text = "Не все поля заполнены";
            _errorTextBlock.IsVisible = true;
            return;
        }
        
        var date = _shiftDPicker.SelectedDate.Value.DateTime;
        var startTime = _shiftStartTPicker.SelectedTime.Value;
        var endTime = _shiftEndTPicker.SelectedTime.Value;
        
        var fullStartDate = date.Add(startTime);
        var fullEndDate = date.Add(endTime);
        
        _editShift.ShiftStarted = fullStartDate;
        _editShift.ShiftEnds = fullEndDate;
        
        var selectedUsers = Users.Where(x => x.IsSelected).Select(x => x.User).ToList();

        if (selectedUsers.Count < 4 || selectedUsers.Count > 7)
        {
            _errorTextBlock.Text = "Сотрудников на смене должно быть от 4 до 7";
            _errorTextBlock.IsVisible = true;
            return;
        }
        
        _editShift.Users = selectedUsers;
        
        if (_editShift.Id != 0)
            _db.Shifts.Update(_editShift);
        else
            await _db.Shifts.AddAsync(_editShift);
        
        await _db.SaveChangesAsync();

        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        var selectedUser = _shiftUsersDataGrid.SelectedItem as SelectionUser;
        
        if (selectedUser == null)
            return;
        
        var user = Users.First(x => x.User.Id == selectedUser.User.Id);
        
        var checkBox = sender as CheckBox;
        user.IsSelected = checkBox?.IsChecked ?? false;
        
        Users.Remove(selectedUser);
        Users.Add(user);
        
        _shiftUsersDataGrid.ItemsSource = Users;
    }
}