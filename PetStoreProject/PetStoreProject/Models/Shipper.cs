﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Shipper")]
public partial class Shipper
{
    [Key]
    public int ShipperId { get; set; }

    [StringLength(250)]
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
    [InverseProperty("Shippers")]
    public virtual Account Account { get; set; } = null!;

    [InverseProperty("Shipper")]
    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    [InverseProperty("Shipper")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
