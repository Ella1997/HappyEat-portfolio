using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class Food
{
    public int FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public string? FoodCategory { get; set; }

    public decimal Calories { get; set; }

    public decimal? Carbs { get; set; }

    public decimal? Protein { get; set; }

    public decimal? Fat { get; set; }

    public virtual ICollection<FoodRecordItem> FoodRecordItems { get; set; } = new List<FoodRecordItem>();
}
