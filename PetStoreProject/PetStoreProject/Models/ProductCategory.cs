using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class ProductCategory
{
    public int ProductCateId { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
