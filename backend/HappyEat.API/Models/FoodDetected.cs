using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class FoodDetected
{
    public int DetectId { get; set; }

    public int ImageId { get; set; }

    public string? EstimatedFood { get; set; }

    public decimal? EstimatedWeight { get; set; }

    public decimal EstimatedCalories { get; set; }

    public decimal? EstimatedCarbs { get; set; }

    public decimal? EstimatedProtein { get; set; }

    public decimal? EstimatedFat { get; set; }

    public string? Ainote { get; set; }

    public virtual FoodImage Image { get; set; } = null!;
}
