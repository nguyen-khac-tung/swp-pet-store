﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("WorkingTime")]
public partial class WorkingTime
{
    [Key]
    public int WorkingTimeId { get; set; }

    [Precision(0)]
    public TimeOnly Time { get; set; }

    [InverseProperty("WorkingTime")]
    public virtual ICollection<TimeService> TimeServices { get; set; } = new List<TimeService>();
}
