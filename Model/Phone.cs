﻿using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Phone
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public string? Description { get; set; }

    public string Image { get; set; } = null!;

    public virtual ICollection<Phoneoffer> Phoneoffers { get; } = new List<Phoneoffer>();
}
