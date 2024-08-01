﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("District")]
public partial class District
{
    [Key]
    public int DistrictId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int ShipperId { get; set; }

    [ForeignKey("ShipperId")]
    [InverseProperty("Districts")]
    public virtual Shipper Shipper { get; set; } = null!;
}
