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

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int CreateBy { get; set; }

    public byte[]? CreateAt { get; set; }

    public int? BrandId { get; set; }

    public int? ProductCateId { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Promotions")]
    public virtual Brand? Brand { get; set; }

    [ForeignKey("CreateBy")]
    [InverseProperty("Promotions")]
    public virtual Admin CreateByNavigation { get; set; } = null!;

    [InverseProperty("Promotion")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("ProductCateId")]
    [InverseProperty("Promotions")]
    public virtual ProductCategory? ProductCate { get; set; }
}
