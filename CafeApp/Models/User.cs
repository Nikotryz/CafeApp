using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Status { get; set; } = null!;

    public byte[]? UserPhoto { get; set; }

    public byte[]? ContractPhoto { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
