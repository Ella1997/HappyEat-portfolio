using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class Drink
{
    public int DrinkId { get; set; }

    public string? DrinkName { get; set; }

    public string? Size { get; set; }

    public string? DrinkCategory { get; set; }

    public decimal? Calories { get; set; }

    public decimal? Carbs { get; set; }

    public decimal? Protein { get; set; }

    public decimal? Fat { get; set; }

    public virtual ICollection<FoodRecordItem> FoodRecordItems { get; set; } = new List<FoodRecordItem>();
}
