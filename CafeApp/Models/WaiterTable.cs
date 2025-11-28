using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public partial class WaiterTable
{
    public int ShiftId { get; set; }

    public int UserId { get; set; }

    public int TableId { get; set; }

    public virtual Shift Shift { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
