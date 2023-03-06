using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Storetouser
{
    public int A { get; set; }

    public int B { get; set; }

    public virtual Store ANavigation { get; set; } = null!;

    public virtual User BNavigation { get; set; } = null!;
}
