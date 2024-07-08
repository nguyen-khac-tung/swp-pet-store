﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Discount")]
public partial class Discount
{
    [Key]
    public int DiscountId { get; set; }

    [StringLength(15)]
    public string? Code { get; set; }

    public int? DiscountTypeId { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? Value { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? MaxValue { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? MinPurchase { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? Quantity { get; set; }

    public int? MaxUse { get; set; }

    public int? Used { get; set; }

    public string? Description { get; set; }

    [StringLength(25)]
    public string? CreatedAt { get; set; }

    public bool? Status { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Discounts")]
    public virtual Admin? CreatedByNavigation { get; set; }

    [ForeignKey("DiscountTypeId")]
    [InverseProperty("Discounts")]
    public virtual DiscountType? DiscountType { get; set; }

    [InverseProperty("Discount")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}