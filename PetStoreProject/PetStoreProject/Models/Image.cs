using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
}
