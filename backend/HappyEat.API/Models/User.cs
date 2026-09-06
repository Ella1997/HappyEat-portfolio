using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Gender { get; set; }

    public virtual ICollection<BodyRecord> BodyRecords { get; set; } = new List<BodyRecord>();

    public virtual ICollection<FoodImage> FoodImages { get; set; } = new List<FoodImage>();

    public virtual ICollection<FoodRecord> FoodRecords { get; set; } = new List<FoodRecord>();

    public virtual ICollection<UserGoal> UserGoals { get; set; } = new List<UserGoal>();
}
