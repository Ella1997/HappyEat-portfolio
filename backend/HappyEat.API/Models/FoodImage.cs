using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class FoodImage
{
    public int ImageId { get; set; }

    public int UserId { get; set; }

    public string? ImagePath { get; set; }

    public DateTime UploadTime { get; set; }

    public virtual FoodRecord? FoodRecord { get; set; }

    public virtual User User { get; set; } = null!;
}
