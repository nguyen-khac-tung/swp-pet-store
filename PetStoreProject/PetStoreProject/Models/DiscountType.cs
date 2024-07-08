﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("DiscountType")]
public partial class DiscountType
{
    [Key]
    public int DiscountTypeId { get; set; }

    [StringLength(250)]
    public string? DiscountName { get; set; }

    [InverseProperty("DiscountType")]
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}