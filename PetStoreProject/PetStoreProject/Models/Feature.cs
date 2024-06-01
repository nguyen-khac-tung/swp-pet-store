using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Feature
{
    public int FeatureId { get; set; }

    public string Url { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
