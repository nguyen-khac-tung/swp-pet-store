using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateOnly DatePosted { get; set; }

    public int EmployeeId { get; set; }
}
