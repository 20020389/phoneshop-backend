using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class File
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Path { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime UpdateAt { get; set; }

    public DateTime CreateAt { get; set; }
}
