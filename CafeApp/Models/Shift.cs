using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public class Shift
{
    public int Id { get; set; }

    public DateTime ShiftStarted { get; set; }

    public DateTime ShiftEnds { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<WaiterTable> WaiterTables { get; set; } = new List<WaiterTable>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
