﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("Employee")]
public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [StringLength(250)]
    public string FullName { get; set; } = null!;

    public bool Gender { get; set; }

    public DateOnly DoB { get; set; }

    [StringLength(50)]
    public string Phone { get; set; } = null!;

    [StringLength(250)]
    public string Address { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    public bool? IsDelete { get; set; }

    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Employees")]
    public virtual Account Account { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    [InverseProperty("Employee")]
    public virtual ICollection<News> News { get; set; } = new List<News>();

    [InverseProperty("Employee")]
    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();

    [InverseProperty("Employee")]
    public virtual ICollection<ResponseFeedback> ResponseFeedbacks { get; set; } = new List<ResponseFeedback>();
}
