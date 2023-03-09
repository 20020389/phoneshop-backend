using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Store
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string Group { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
