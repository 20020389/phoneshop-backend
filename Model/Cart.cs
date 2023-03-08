using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Cart
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public virtual ICollection<Stringtemplate> Stringtemplates { get; } = new List<Stringtemplate>();

    public virtual User User { get; set; } = null!;
}
