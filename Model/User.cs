using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class User
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Name { get; set; }

    public string? Image { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Profile { get; set; }

    public string Role { get; set; } = null!;

    public bool Verified { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime LastLogin { get; set; }

    public DateTime RegisteredAt { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Phonerating> Phoneratings { get; } = new List<Phonerating>();

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
