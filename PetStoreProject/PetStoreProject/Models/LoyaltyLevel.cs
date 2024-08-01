using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

public partial class LoyaltyLevel
{
    [Key]
    [Column("LevelID")]
    public int LevelId { get; set; }

    [StringLength(100)]
    public string? LevelName { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? MinTotalAmount { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? MaxTotalAmount { get; set; }
}
