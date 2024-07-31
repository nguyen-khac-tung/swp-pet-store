﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("ResponseFeedback")]
public partial class ResponseFeedback
{
    [Key]
    public int ResponseId { get; set; }

    public int FeedbackId { get; set; }

    public int EmployeeId { get; set; }

    [StringLength(500)]
    public string? Content { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("ResponseFeedbacks")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("FeedbackId")]
    [InverseProperty("ResponseFeedbacks")]
    public virtual Feedback Feedback { get; set; } = null!;
}
