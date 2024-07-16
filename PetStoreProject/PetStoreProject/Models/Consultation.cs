﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Consultation")]
public partial class Consultation
{
    [Key]
    public int ConsultationId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(150)]
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateOnly Date { get; set; }

    public bool Status { get; set; }

    public int? EmployeeId { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    public string? Response { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Consultations")]
    public virtual Employee? Employee { get; set; }
}
