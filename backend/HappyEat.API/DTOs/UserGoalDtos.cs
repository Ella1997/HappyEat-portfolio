namespace HappyEat.API.DTOs
{
    public class UserGoalCreateDto
    {
        public int UserId { get; set; }
        public string GoalType { get; set; } = null!;
        public decimal? StartWeight { get; set; }
        public decimal? TargetWeight { get; set; }
        public decimal? TargetBodyFat { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? TargetDate { get; set; }
    }

    public class UserGoalUpdateDto 
    {
        public string GoalType { get; set; } = null!;
        public decimal? TargetWeight { get; set; }
        public decimal? TargetBodyFat { get; set; }
        public DateOnly? TargetDate { get; set; }
    }

    public class UserGoalResponseDto
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
    }
}
