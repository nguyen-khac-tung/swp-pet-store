﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("ReturnRefund")]
public partial class ReturnRefund
{
    [Key]
    public int ReturnId { get; set; }

    public string ReasonReturn { get; set; } = null!;

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public string? ResponseContent { get; set; }

    [InverseProperty("Return")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("Return")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
