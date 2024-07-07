﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("OrderItem")]
public partial class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductOptionId { get; set; }

    public int Quantity { get; set; }

    public float Price { get; set; }

    public int? PromotionId { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductOptionId")]
    [InverseProperty("OrderItems")]
    public virtual ProductOption ProductOption { get; set; } = null!;

    [ForeignKey("PromotionId")]
    [InverseProperty("OrderItems")]
    public virtual Promotion? Promotion { get; set; }
}
