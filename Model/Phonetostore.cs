using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Phonetostore
{
    public int A { get; set; }

    public int B { get; set; }

    public virtual Phone ANavigation { get; set; } = null!;

    public virtual Store BNavigation { get; set; } = null!;
}
