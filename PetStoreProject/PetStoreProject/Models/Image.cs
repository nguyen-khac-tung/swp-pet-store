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

    [InverseProperty("Image")]
    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
}
