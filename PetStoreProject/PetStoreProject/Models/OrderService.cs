﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("OrderService")]
public partial class OrderService
{
    [Key]
    public int OrderServiceId { get; set; }

    public int? CustomerId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Phone { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    [Precision(0)]
    public TimeOnly OrderTime { get; set; }

    public DateOnly DateCreated { get; set; }

    public int ServiceOptionId { get; set; }

    public float? Price { get; set; }

    [StringLength(250)]
    public string? Message { get; set; }

    [StringLength(30)]
    public string Status { get; set; } = null!;

    public bool IsDelete { get; set; }

    public int? EmployeeId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("OrderServices")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("OrderServices")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("ServiceOptionId")]
    [InverseProperty("OrderServices")]
    public virtual ServiceOption ServiceOption { get; set; } = null!;
}
