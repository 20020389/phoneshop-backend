using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Transaction
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? UserId { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime CreateAt { get; set; }

    public int StoreId { get; set; }

    public virtual Store Store { get; set; } = null!;

    public virtual ICollection<Stringtemplate> Stringtemplates { get; } = new List<Stringtemplate>();

    public virtual User? User { get; set; }
}
