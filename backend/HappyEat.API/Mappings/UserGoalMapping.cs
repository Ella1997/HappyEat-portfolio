using HappyEat.API.DTOs;
using HappyEat.API.Models;

namespace HappyEat.API.Mappings
{
    public static class UserGoalMapping
    {
        //DTO->Entity
        public static UserGoal ToEntity(this UserGoalCreateDto dto)
        {
            return new UserGoal
            {
                UserId = dto.UserId,
                GoalType = dto.GoalType,
                StartWeight = dto.StartWeight,
                TargetWeight = dto.TargetWeight,
                TargetBodyFat = dto.TargetBodyFat,
                StartDate = dto.StartDate,
                TargetDate = dto.TargetDate,
                IsActive = true,
            };
        }
        //Entity->Response Dto
        public static UserGoalResponseDto ToResponseDto(this UserGoal goal)
        {
            return new UserGoalResponseDto
            {
                GoalId = goal.GoalId,
                UserId = goal.UserId,
                GoalType = goal.GoalType,
                StartWeight = goal.StartWeight,
                TargetWeight = goal.TargetWeight,
                TargetBodyFat = goal.TargetBodyFat,
                StartDate = goal.StartDate,
                TargetDate = goal.TargetDate,
                IsActive = goal.IsActive,
            };
        }
    }
}
