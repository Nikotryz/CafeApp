using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public partial class Order
{
    public int Id { get; set; }

    public int ShiftId { get; set; }

    public int TableId { get; set; }

    public int ClientsAmount { get; set; }

    public string Content { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public TimeOnly CreatedAt { get; set; }

    public TimeOnly? CompletedAt { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual Shift Shift { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
