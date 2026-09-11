using HappyEat.API.Data;
using HappyEat.API.DTOs;
using HappyEat.API.Exceptions;
using HappyEat.API.Mappings;
using HappyEat.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyEat.API.Services
{
    public class UserGoalService : IUserGoalService
    {
        private readonly HappyEatDbContext _dbContext;
        public UserGoalService(HappyEatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<UserGoalResponseDto> CreateAsync(UserGoalCreateDto dto)
        {
            if (dto.IsActive)
            {
                var activeGoals = await _dbContext.UserGoals.Where(g => g.UserId == dto.UserId && g.IsActive).ToListAsync();
                foreach(var goal in activeGoals)
                {
                    goal.IsActive = false;
                }
            }

            var newGoal = dto.ToEntity();
            _dbContext.UserGoals.Add(newGoal);
            await _dbContext.SaveChangesAsync();
            return newGoal.ToResponseDto();
        }

        public async Task DeleteAsync(int userId, int goalId)
        {
            var goal = await _dbContext.UserGoals.AsNoTracking()
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GoalId == goalId);

            if (goal == null) throw new NotFoundException("Goal Not Found.");

            _dbContext.UserGoals.Remove(goal);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserGoalResponseDto> GetByIdAsync(int userId, int goalId)
        {
            var goal = await _dbContext.UserGoals.AsNoTracking()
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GoalId == goalId);

            if (goal == null) throw new NotFoundException("Goal Not Found.");

            return goal.ToResponseDto();
        }

        public async Task<List<UserGoalResponseDto>> GetByUserIdAsync(int userId)
        {
            var goals = await _dbContext.UserGoals.AsNoTracking()
                .Where(g => g.UserId == userId).ToListAsync();

            return goals.Select(g => g.ToResponseDto()).ToList();
        }

        public async Task UpdateAsync(int userId, int goalId, UserGoalUpdateDto dto)
        {
            var goal = await _dbContext.UserGoals.AsNoTracking()
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GoalId == goalId);

            if (goal == null) throw new NotFoundException("Goal Not Found.");

            if (dto.IsActive)
            {
                var activeGoals = await _dbContext.UserGoals.Where(g => g.UserId == userId && g.GoalId!=goalId && g.IsActive).ToListAsync();
                foreach (var activeGoal in activeGoals)
                {
                    activeGoal.IsActive = false;
                }
            }

            goal.GoalType = dto.GoalType;
            goal.TargetWeight = dto.TargetWeight;
            goal.TargetBodyFat = dto.TargetBodyFat;
            goal.StartDate = dto.StartDate;
            goal.TargetDate = dto.TargetDate;
            goal.IsActive = dto.IsActive;

            await _dbContext.SaveChangesAsync();
        }
    }
}
