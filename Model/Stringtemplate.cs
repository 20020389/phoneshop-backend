using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Stringtemplate
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public int? TransactionId { get; set; }

    public virtual Transaction? Transaction { get; set; }
}
