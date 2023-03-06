using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Phoneoffer
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public int Price { get; set; }

    public string Color { get; set; } = null!;

    public string Storage { get; set; } = null!;

    public string PhoneId { get; set; } = null!;

    public virtual Phone Phone { get; set; } = null!;
}
