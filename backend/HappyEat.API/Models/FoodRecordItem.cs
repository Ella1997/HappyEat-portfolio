using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class FoodRecordItem
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public int RecordId { get; set; }

    public int? FoodId { get; set; }

    public int? DrinkId { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal Calories { get; set; }

    public decimal? Carbs { get; set; }

    public decimal? Protein { get; set; }

    public decimal? Fat { get; set; }

    public virtual Drink? Drink { get; set; }

    public virtual Food? Food { get; set; }

    public virtual FoodRecord Record { get; set; } = null!;
}
