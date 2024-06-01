using System;
using System.Collections.Generic;

namespace PetStoreProject.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Content { get; set; }

    public DateTime DateCreated { get; set; }
}
