using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class ShiftEditWindow : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();

    private DatePicker shiftDPicker;
    private TimePicker shiftStartTPicker;
    private TimePicker shiftEndTPicker;
    private ListBox usersLBox;
    
    public List<User> Users { get; set; }
    
    public ShiftEditWindow()
    {
        InitializeComponent();
        
        shiftDPicker = this.FindControl<DatePicker>("ShiftDPicker")!;
        shiftStartTPicker = this.FindControl<TimePicker>("ShiftStartTPicker")!;
        shiftEndTPicker = this.FindControl<TimePicker>("ShiftEndTPicker")!;
        // usersLBox = this.FindControl<ListBox>("UsersLBox")!;
        
        // usersLBox.ItemsSource = _db.Users.Include(x => x.Role).ToList();
    }
}