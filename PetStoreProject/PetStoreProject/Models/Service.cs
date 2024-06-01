using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<ServiceOption> ServiceOptions { get; set; } = new List<ServiceOption>();
}
