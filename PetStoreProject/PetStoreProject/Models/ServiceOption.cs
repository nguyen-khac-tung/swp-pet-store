﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("ServiceOption")]
public partial class ServiceOption
{
    [Key]
    public int ServiceOptionId { get; set; }

    public int ServiceId { get; set; }

    [StringLength(10)]
    public string PetType { get; set; } = null!;

    [StringLength(30)]
    public string Weight { get; set; } = null!;

    public float Price { get; set; }

    public bool IsDelete { get; set; }

    [InverseProperty("ServiceOption")]
    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();

    [ForeignKey("ServiceId")]
    [InverseProperty("ServiceOptions")]
    public virtual Service Service { get; set; } = null!;
}
