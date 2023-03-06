using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Storetotransaction
{
    public int A { get; set; }

    public int B { get; set; }

    public virtual Store ANavigation { get; set; } = null!;

    public virtual Transaction BNavigation { get; set; } = null!;
}
