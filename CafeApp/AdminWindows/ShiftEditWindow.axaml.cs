using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    public List<SelectionUser> Users { get; set; } = [];
    
    public ShiftEditWindow()
    {
        InitializeComponent();
        
        _shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        _shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        _shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        _shiftUsersDataGrid = this.FindControl<DataGrid>("ShiftUsersDataGrid")!;
        
        LoadUsers();
    }

    public ShiftEditWindow(Shift shift)
    {
        InitializeComponent();
        
        _shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        _shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        _shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        _shiftUsersDataGrid = this.FindControl<DataGrid>("ShiftUsersDataGrid")!;
        
        _editShift = shift;

        var date = shift.ShiftStarted.Date;
        var startTime = shift.ShiftStarted.TimeOfDay;
        var endTime = shift.ShiftEnds.TimeOfDay;
        var users = shift.Users.ToList();
        
        _shiftDPicker.SelectedDate = date;
        _shiftStartTPicker.SelectedTime = startTime;
        _shiftEndTPicker.SelectedTime = endTime;
        
        LoadUsers();
    }

    private void LoadUsers()
    {
        var allUsers = _db.Users.Include(x => x.Role).Include(x => x.Shifts).ToList();
        var selectedUsers = new List<SelectionUser>();

        foreach (var user in allUsers)
        {
            selectedUsers.Add(new SelectionUser { User = user, IsSelected = user.Shifts.Contains(_editShift) });
        }
        
        _shiftUsersDataGrid.ItemsSource = selectedUsers;
    }

    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var date = _shiftDPicker.SelectedDate!.Value.DateTime;
        var startTime = _shiftStartTPicker.SelectedTime!.Value;
        var endTime = _shiftEndTPicker.SelectedTime!.Value;
        
        var fullStartDate = date.Add(startTime);
        var fullEndDate = date.Add(endTime);
        
        _editShift.ShiftStarted = fullStartDate;
        _editShift.ShiftEnds = fullEndDate;
        _editShift.Users = Users.Where(x => x.IsSelected).Select(x => x.User).ToList();
        
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
}