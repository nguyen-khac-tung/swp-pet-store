using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class ServiceOption
{
    public int ServiceId { get; set; }

    public string PetType { get; set; } = null!;

    public float Price { get; set; }

    public virtual Service Service { get; set; } = null!;
}
