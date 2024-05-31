using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int BrandId { get; set; }

    public int ProductCateId { get; set; }

    public string? Description { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ProductCategory ProductCate { get; set; } = null!;

    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
