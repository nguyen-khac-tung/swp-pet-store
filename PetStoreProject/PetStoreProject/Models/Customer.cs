using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public bool? Gender { get; set; }

    public string? Phone { get; set; }

    public DateOnly? DoB { get; set; }

    public string? Address { get; set; }

    public string Email { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
