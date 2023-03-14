using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Phone
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int Quantity { get; set; }

    public string? Images { get; set; }

    public string? Tags { get; set; }

    public string? Profile { get; set; }

    public string? Description { get; set; }

    public string? Detail { get; set; }

    public string? Sold { get; set; }

    public double Rating { get; set; }

    public string StoreId { get; set; } = null!;

    public DateTime UpdateAt { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual ICollection<Phoneoffer> Phoneoffers { get; } = new List<Phoneoffer>();

    public virtual ICollection<Phonerating> Phoneratings { get; } = new List<Phonerating>();

    public virtual Store Store { get; set; } = null!;
}
