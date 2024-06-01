using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int ProductOptionId { get; set; }

    public int Quantity { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ProductOption ProductOption { get; set; } = null!;
}
