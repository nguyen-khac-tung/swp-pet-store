﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Customer")]
public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required]
    [StringLength(250)]
    public string FullName { get; set; }

    public bool? Gender { get; set; }

    [StringLength(50)]
    public string Phone { get; set; }

    public DateOnly? DoB { get; set; }

    [StringLength(250)]
    public string Address { get; set; }

    [Required]
    [StringLength(150)]
    public string Email { get; set; }

    public bool? IsDelete { get; set; }

    public int? LoyaltyLevelID { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TotalAmountSpent { get; set; }

    public int AccountId { get; set; }

    public bool? isLoyalty { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Customers")]
    public virtual Account Account { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("LoyaltyLevelID")]
    [InverseProperty("Customers")]
    public virtual LoyaltyLevel LoyaltyLevel { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("CustomerId")]
    [InverseProperty("Customers")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}