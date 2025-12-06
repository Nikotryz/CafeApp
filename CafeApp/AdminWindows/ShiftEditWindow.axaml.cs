using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.AdminWindows;

public partial class ShiftEditWindow : Window
{
    private readonly Shift? _editShift;
    
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();

    private readonly DatePicker _shiftDPicker;
    private readonly TimePicker _shiftStartTPicker;
    private readonly TimePicker _shiftEndTPicker;
    
    private readonly DataGrid _shiftUsersDataGrid;
    private readonly DataGrid _waiterTablesDataGrid;
    
    private readonly Button _deleteWaiterTableBtn;
    
    private readonly TextBlock _errorTextBlock;

    public List<SelectionUser> Users { get; set; } = [];
    public List<WaiterTable> WaiterTables { get; set; } = [];
    
    public ShiftEditWindow()
    {
        InitializeComponent();
        
        _shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        _shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        _shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        
        _shiftUsersDataGrid = this.FindControl<DataGrid>("ShiftUsersDataGrid")!;
        _waiterTablesDataGrid = this.FindControl<DataGrid>("WaiterTablesDataGrid")!;
        
        _deleteWaiterTableBtn = this.FindControl<Button>("DeleteWaiterTableBtn")!;
        
        _errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock")!;
        
        LoadUsers();
    }

    // : this() означает что при вызове этого конструктора сначала будет вызван базовый конструктор (который выше)
    public ShiftEditWindow(Shift shift) : this()
    {
        _editShift = shift;

        var date = shift.ShiftStarted.Date;
        var startTime = shift.ShiftStarted.TimeOfDay;
        var endTime = shift.ShiftEnds.TimeOfDay;
        
        _shiftDPicker!.SelectedDate = date;
        _shiftStartTPicker!.SelectedTime = startTime;
        _shiftEndTPicker!.SelectedTime = endTime;
        
        WaiterTables = _db.WaiterTables.Where(x => x.ShiftId == shift.Id).ToList();
        
        LoadUsers();
        LoadWaiterTables();
    }

    private void LoadUsers()
    {
        var allUsers = _db.Users
            .Include(x => x.Role)
            .Include(x => x.Shifts)
            .Where(x => x.Status == UserStatuses.USER_WORKED)
            .OrderBy(x => x.Role)
            .ToList();
        
        var selectionUsers = new List<SelectionUser>();

        foreach (var user in allUsers)
            selectionUsers.Add(new SelectionUser { User = user, IsSelected = user.Shifts.Contains(_editShift) });

        Users = selectionUsers;
        _shiftUsersDataGrid.ItemsSource = selectionUsers;
    }

    // => просто сокращенная запись метода в одну строчку
    private void LoadWaiterTables() => _waiterTablesDataGrid.ItemsSource = WaiterTables.ToList();

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

    private void WaiterTablesDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => _deleteWaiterTableBtn.IsEnabled = _waiterTablesDataGrid.SelectedItem != null;

    private void DeleteWaiterTableBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedWaiterTable = _waiterTablesDataGrid.SelectedItem as WaiterTable;
        if (selectedWaiterTable != null)
            WaiterTables.Remove(selectedWaiterTable);
        
        LoadWaiterTables();
    }

    private async void AddWaiterTableBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var waiterTable = await new AddWaiterTableWindow().ShowDialog<WaiterTable?>(this);

        if (waiterTable != null && !WaiterTables.Any(x => x.Table == waiterTable.Table && x.User == waiterTable.User))
            WaiterTables.Add(waiterTable);
        
        LoadWaiterTables();
    }

    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var shift = _editShift ?? new Shift();
        
        if (_shiftDPicker.SelectedDate == null ||
            _shiftStartTPicker.SelectedTime == null ||
            _shiftEndTPicker.SelectedTime == null)
        {
            ShowMessage("Не все поля заполнены");
            return;
        }
        
        var date = _shiftDPicker.SelectedDate.Value.DateTime;
        var startTime = _shiftStartTPicker.SelectedTime.Value;
        var endTime = _shiftEndTPicker.SelectedTime.Value;

        if (date < DateTime.Today.ToLocalTime())
        {
            ShowMessage("Нельзя сделать смену на прошедшую дату");
            return;
        }
        
        if (date > DateTime.Today.AddDays(5).ToLocalTime())
        {
            ShowMessage("Смены можно делать не более чем на 5 дней вперед");
            return;
        }
        
        var fullStartDate = date.Add(startTime);
        var fullEndDate = date.Add(endTime);

        if (fullStartDate >= fullEndDate)
        {
            ShowMessage("Смена заканчивается раньше чем начинается");
            return;
        }
        
        shift.ShiftStarted = fullStartDate;
        shift.ShiftEnds = fullEndDate;
        
        var selectedUsers = Users.Where(x => x.IsSelected).Select(x => x.User).ToList();

        if (selectedUsers.Count < 4 || selectedUsers.Count > 7)
        {
            ShowMessage("Сотрудников на смене должно быть от 4 до 7");
            return;
        }
        
        shift.Users = selectedUsers;
        
        if (_editShift != null)
            _db.Shifts.Update(shift);
        else
            await _db.Shifts.AddAsync(shift);

        await _db.SaveChangesAsync();
        
        _db.WaiterTables.RemoveRange(_db.WaiterTables.Where(x => x.ShiftId == shift.Id));
        
        await _db.SaveChangesAsync();
        
        foreach (var waiterTable in WaiterTables)
        {
            waiterTable.Shift = shift;
            await _db.WaiterTables.AddAsync(waiterTable);
        }
        
        await _db.SaveChangesAsync();

        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e) => Close();

    private void ShowMessage(string message)
    {
        _errorTextBlock.Text = message;
        _errorTextBlock.IsVisible = true;
    }
}