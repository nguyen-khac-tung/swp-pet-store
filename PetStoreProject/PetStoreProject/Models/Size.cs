using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
}
