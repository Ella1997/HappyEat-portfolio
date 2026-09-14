using System;
using System.Collections.Generic;

namespace HappyEat.API.Models;

public partial class UserGoal
{
    public int GoalId { get; set; }

    public int UserId { get; set; }

    public string GoalType { get; set; } = null!;

    public decimal? TargetWeight { get; set; }

    public decimal? TargetBodyFat { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? TargetDate { get; set; }

    public bool IsActive { get; set; }

    public decimal? StartWeight { get; set; }

    public virtual User User { get; set; } = null!;
}
