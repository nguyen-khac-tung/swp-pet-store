using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class ProductOption
{
    public int ProductOptionId { get; set; }

    public int ProductId { get; set; }

    public int? SizeId { get; set; }

    public int? AttributeId { get; set; }

    public float Price { get; set; }

    public bool? IsSoldOut { get; set; }

    public int ImageId { get; set; }

    public virtual Attribute? Attribute { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Image Image { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Size? Size { get; set; }
}
