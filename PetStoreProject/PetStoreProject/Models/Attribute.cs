using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Attribute
{
    public int AttributeId { get; set; }

    public string? Type { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
}
