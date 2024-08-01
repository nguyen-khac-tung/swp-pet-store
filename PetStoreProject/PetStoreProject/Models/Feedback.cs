﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    public int FeedbackId { get; set; }

    public int? CustomerId { get; set; }

    public int? ProductId { get; set; }

    public int? ServiceId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Phone { get; set; } = null!;

    [StringLength(150)]
    public string? Email { get; set; }

    public int Rating { get; set; }

    [StringLength(500)]
    public string Content { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    public bool Status { get; set; }

    public int? ResponseId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Feedbacks")]
    public virtual Product? Product { get; set; }

    [InverseProperty("Feedback")]
    public virtual ICollection<ResponseFeedback> ResponseFeedbacks { get; set; } = new List<ResponseFeedback>();

    [ForeignKey("ServiceId")]
    [InverseProperty("Feedbacks")]
    public virtual Service? Service { get; set; }
}