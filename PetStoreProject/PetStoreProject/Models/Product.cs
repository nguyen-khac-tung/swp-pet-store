﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    public int BrandId { get; set; }

    public int ProductCateId { get; set; }

    public string? Description { get; set; }

    public bool? IsDelete { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand Brand { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [ForeignKey("ProductCateId")]
    [InverseProperty("Products")]
    public virtual ProductCategory ProductCate { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

    [ForeignKey("ProductId")]
    [InverseProperty("Products")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}