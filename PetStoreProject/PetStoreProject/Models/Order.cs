﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

public partial class Order
{
    [Key]
    public long OrderId { get; set; }

    public int? CustomerId { get; set; }

    [StringLength(250)]
    public string FullName { get; set; } = null!;

    [StringLength(50)]
    public string Phone { get; set; } = null!;

    [StringLength(150)]
    public string? Email { get; set; }

    public float TotalAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [StringLength(250)]
    public string ConsigneeFullName { get; set; }

    [StringLength(50)]
    public string ConsigneePhone { get; set; }

    [StringLength(50)]
    public string PaymetMethod { get; set; } = null!;

    [StringLength(250)]
    public string ShipAddress { get; set; } = null!;

    public int? DiscountId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Orders")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("Orders")]
    public virtual Discount? Discount { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
