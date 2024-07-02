﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("ProductCategory")]
public partial class ProductCategory
{
    [Key]
    public int ProductCateId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public bool? IsDelete { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ProductCategories")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("ProductCate")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("ProductCate")]
    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
