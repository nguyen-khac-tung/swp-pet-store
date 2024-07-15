using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Consultation
{
    public int ConsultationId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateOnly Date { get; set; }

    public bool Status { get; set; }

    public int? EmployeeId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Response { get; set; }

    public virtual Employee? Employee { get; set; }
}
