﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Image")]
public partial class Image
{
    [Key]
    public int ImageId { get; set; }

    [StringLength(250)]
    public string ImageUrl { get; set; } = null!;

    public int? ServiceId { get; set; }

    public int? NewsId { get; set; }

    public long? OrderId { get; set; }

    public int? ReturnId { get; set; }

    [ForeignKey("NewsId")]
    [InverseProperty("Images")]
    public virtual News? News { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Images")]
    public virtual Order? Order { get; set; }

    [InverseProperty("Image")]
    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

    [ForeignKey("ReturnId")]
    [InverseProperty("Images")]
    public virtual ReturnRefund? Return { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("Images")]
    public virtual Service? Service { get; set; }
}
