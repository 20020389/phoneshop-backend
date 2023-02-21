using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Phone
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int Price { get; set; }
}
