﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
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

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Phone { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    public int ServiceOptionId { get; set; }

    [StringLength(250)]
    public string? Message { get; set; }

    [StringLength(30)]
    public string Status { get; set; } = null!;

    public bool IsDelete { get; set; }

    public int? EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("OrderServices")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("ServiceOptionId")]
    [InverseProperty("OrderServices")]
    public virtual ServiceOption ServiceOption { get; set; } = null!;
}