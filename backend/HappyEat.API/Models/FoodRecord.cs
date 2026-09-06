using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class FoodRecord
{
    public int RecordId { get; set; }

    public int UserId { get; set; }

    public int? ImageId { get; set; }

    public string RecordSource { get; set; } = null!;

    public DateTime RecordDate { get; set; }

    public string? MealType { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<FoodRecordItem> FoodRecordItems { get; set; } = new List<FoodRecordItem>();

    public virtual FoodImage? Image { get; set; }

    public virtual User User { get; set; } = null!;
}
