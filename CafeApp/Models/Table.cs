using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public partial class Table
{
    public int Id { get; set; }

    public int Number { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
