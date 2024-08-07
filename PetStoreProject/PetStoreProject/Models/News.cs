﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

public partial class News
{
    [Key]
    public int NewsId { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateOnly DatePosted { get; set; }

    public int? TagId { get; set; }

    [StringLength(250)]
    public string? Summary { get; set; }

    public bool? IsDelete { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("News")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("News")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [ForeignKey("TagId")]
    [InverseProperty("News")]
    public virtual TagNews? Tag { get; set; }
}