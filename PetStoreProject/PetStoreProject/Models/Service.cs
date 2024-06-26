﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Service")]
public partial class Service
{
    [Key]
    public int ServiceId { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(30)]
    public string? Type { get; set; }

    public string? SupDescription { get; set; }

    public string? Description { get; set; }

    public bool IsDelete { get; set; }

    [InverseProperty("Service")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Service")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("Service")]
    public virtual ICollection<ServiceOption> ServiceOptions { get; set; } = new List<ServiceOption>();

    [InverseProperty("Service")]
    public virtual ICollection<TimeService> TimeServices { get; set; } = new List<TimeService>();
}