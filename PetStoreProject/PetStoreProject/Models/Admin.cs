﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Admin")]
public partial class Admin
{
    [Key]
    public int AdminId { get; set; }

    [StringLength(1000)]
    public string FullName { get; set; } = null!;

    public bool Gender { get; set; }

    public DateOnly DoB { get; set; }

    [StringLength(50)]
    public string Phone { get; set; } = null!;

    [StringLength(250)]
    public string Address { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    public bool? IsDelete { get; set; }

    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Admins")]
    public virtual Account Account { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    [InverseProperty("CreateByNavigation")]
    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
