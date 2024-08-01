﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Account")]
public partial class Account
{
    [Key]
    public int AccountId { get; set; }

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(150)]
    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public bool? IsDelete { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    [InverseProperty("Account")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("Account")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [ForeignKey("RoleId")]
    [InverseProperty("Accounts")]
    public virtual Role? Role { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Shipper> Shippers { get; set; } = new List<Shipper>();
}
