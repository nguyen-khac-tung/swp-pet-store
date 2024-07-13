﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[PrimaryKey("WorkingTimeId", "ServiceId")]
[Table("TimeService")]
public partial class TimeService
{
    [Key]
    public int WorkingTimeId { get; set; }

    [Key]
    public int ServiceId { get; set; }

    [StringLength(20)]
    public string? Note { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("TimeServices")]
    public virtual Service Service { get; set; } = null!;

    [ForeignKey("WorkingTimeId")]
    [InverseProperty("TimeServices")]
    public virtual WorkingTime WorkingTime { get; set; } = null!;
}
