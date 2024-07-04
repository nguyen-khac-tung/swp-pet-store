using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Promotion")]
public partial class Promotion
{
    [Key]
    public int PromotionId { get; set; }

    public int? Value { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? MaxValue { get; set; }

    [StringLength(20)]
    public string? StartDate { get; set; }

    [StringLength(20)]
    public string? EndDate { get; set; }

    public int? CreateBy { get; set; }

    public int? BrandId { get; set; }

    public int? ProductCateId { get; set; }

    [StringLength(20)]
    public string? CreatedAt { get; set; }

    [StringLength(250)]
    public string? Name { get; set; }

    [ForeignKey("CreateBy")]
    [InverseProperty("Promotions")]
    public virtual Admin? CreateByNavigation { get; set; }

    [InverseProperty("Promotion")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
