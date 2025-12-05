using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly Birthday { get; set; }

    public string Status { get; set; } = null!;

    public byte[]? UserPhoto { get; set; }

    public byte[]? ContractPhoto { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<WaiterTable> WaiterTables { get; set; } = new List<WaiterTable>();

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
