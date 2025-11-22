using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class ShiftEditWindow : Window
{
    private Shift editShift = new();
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();

    private readonly DatePicker _shiftDPicker;
    private readonly TimePicker _shiftStartTPicker;
    private readonly TimePicker _shiftEndTPicker;
    private readonly ListBox _selectedUsersLBox;
    private readonly ListBox _unselectedUsersLBox;

    public ObservableCollection<User> SelectedUsers { get; set; } = [];
    public ObservableCollection<User> UnselectedUsers { get; set; } = [];
    
    public ShiftEditWindow()
    {
        InitializeComponent();
        
        _shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        _shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        _shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        _selectedUsersLBox = this.FindControl<ListBox>("SelectedUsersLBox")!;
        _unselectedUsersLBox = this.FindControl<ListBox>("UnselectedUsersLBox")!;
        
        LoadUsers();
    }

    public ShiftEditWindow(Shift shift)
    {
        InitializeComponent();
        
        _shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        _shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        _shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        _selectedUsersLBox = this.FindControl<ListBox>("SelectedUsersLBox")!;
        _unselectedUsersLBox = this.FindControl<ListBox>("UnselectedUsersLBox")!;
        
        editShift =  shift;

        var date = shift.ShiftStarted.Date;
        var startTime = shift.ShiftStarted.TimeOfDay;
        var endTime = shift.ShiftEnds.TimeOfDay;
        var users = shift.Users;
        
        _shiftDPicker.SelectedDate = date;
        _shiftStartTPicker.SelectedTime = startTime;
        _shiftEndTPicker.SelectedTime = endTime;
        SelectedUsers = new ObservableCollection<User>(users);
        _selectedUsersLBox.ItemsSource = SelectedUsers;
        
        LoadUsers();
    }

    private void LoadUsers()
    {
        var users = _db.Users.Include(x => x.Role).Where(x => !SelectedUsers.Contains(x)).ToList();
        
        UnselectedUsers = new ObservableCollection<User>(users);
        _unselectedUsersLBox.ItemsSource = UnselectedUsers;
    }

    private void SelectedUsersLBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedUser = _selectedUsersLBox.SelectedItem as User;
        _selectedUsersLBox.SelectedItem = null;

        if (selectedUser == null)
            return;
        
        UnselectedUsers.Add(selectedUser);
        _unselectedUsersLBox.ItemsSource = UnselectedUsers;
        
        SelectedUsers.Remove(selectedUser);
        _selectedUsersLBox.ItemsSource = SelectedUsers;
    }

    private void UnselectedUsersLBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedUser = _unselectedUsersLBox.SelectedItem as User;
        _unselectedUsersLBox.SelectedItem = null;

        if (selectedUser == null)
            return;
        
        SelectedUsers.Add(selectedUser);
        _selectedUsersLBox.ItemsSource = SelectedUsers;

        UnselectedUsers.Remove(selectedUser!);
        _unselectedUsersLBox.ItemsSource = UnselectedUsers;
    }

    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var date = _shiftDPicker.SelectedDate!.Value.DateTime;
        var startTime = _shiftStartTPicker.SelectedTime!.Value;
        var endTime = _shiftEndTPicker.SelectedTime!.Value;
        
        var fullStartDate = date.Add(startTime);
        var fullEndDate = date.Add(endTime);
        
        editShift.ShiftStarted = fullStartDate;
        editShift.ShiftEnds = fullEndDate;
        editShift.Users = SelectedUsers;
        
        if (editShift.Id != 0)
            _db.Shifts.Update(editShift);
        else
            await _db.Shifts.AddAsync(editShift);
        
        await _db.SaveChangesAsync();

        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}