using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public bool Gender { get; set; }

    public DateOnly DoB { get; set; }

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
