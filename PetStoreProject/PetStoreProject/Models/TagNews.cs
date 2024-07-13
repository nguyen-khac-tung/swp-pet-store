﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

public partial class TagNews
{
    [Key]
    public int TagId { get; set; }

    [StringLength(50)]
    public string? TagName { get; set; }

    [InverseProperty("Tag")]
    public virtual ICollection<News> News { get; set; } = new List<News>();
}
