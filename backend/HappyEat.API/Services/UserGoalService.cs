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
            // 驗證起始體重
            if (!dto.StartWeight.HasValue || dto.StartWeight.Value <= 0)
            {
                throw new BusinessException("請輸入有效的起始體重。");
            }

            // 驗證開始日期與目標日期
            ValidateGoalDates(dto.StartDate, dto.TargetDate);

            // 建立新目標前，將原本進行中的目標停用
            var activeGoals = await _dbContext.UserGoals
                .Where(g => g.UserId == dto.UserId && g.IsActive)
                .ToListAsync();

            foreach (var goal in activeGoals)
            {
                goal.IsActive = false;
            }

            var newGoal = dto.ToEntity();

            _dbContext.UserGoals.Add(newGoal);
            await _dbContext.SaveChangesAsync();

            return newGoal.ToResponseDto();
        }

        public async Task EndGoalAsync(int userId, int goalId)
        {
            var goal = await _dbContext.UserGoals
                .FirstOrDefaultAsync(g =>
                    g.UserId == userId &&
                    g.GoalId == goalId &&
                    g.IsActive);

            if (goal == null)
            {
                throw new NotFoundException("找不到進行中的目標。");
            }

            goal.IsActive = false;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserGoalResponseDto> GetByIdAsync(
            int userId,
            int goalId)
        {
            var goal = await _dbContext.UserGoals
                .AsNoTracking()
                .FirstOrDefaultAsync(g =>
                    g.UserId == userId &&
                    g.GoalId == goalId);

            if (goal == null)
            {
                throw new NotFoundException("找不到此目標。");
            }

            return goal.ToResponseDto();
        }

        public async Task<List<UserGoalResponseDto>> GetByUserIdAsync(int userId)
        {
            var goals = await _dbContext.UserGoals
                .AsNoTracking()
                .Where(g => g.UserId == userId)
                .ToListAsync();

            return goals
                .Select(g => g.ToResponseDto())
                .ToList();
        }

        public async Task UpdateAsync(
            int userId,
            int goalId,
            UserGoalUpdateDto dto)
        {
            var goal = await _dbContext.UserGoals
                .FirstOrDefaultAsync(g =>
                    g.UserId == userId &&
                    g.GoalId == goalId &&
                    g.IsActive);

            if (goal == null)
            {
                throw new NotFoundException("找不到進行中的目標。");
            }

            // StartDate 建立後不可修改，
            // 使用原本的 StartDate 驗證新的 TargetDate
            ValidateGoalDates(
                goal.StartDate,
                dto.TargetDate
            );

            goal.GoalType = dto.GoalType;
            goal.TargetWeight = dto.TargetWeight;
            goal.TargetBodyFat = dto.TargetBodyFat;
            goal.TargetDate = dto.TargetDate;

            await _dbContext.SaveChangesAsync();
        }

        private static void ValidateGoalDates(
            DateOnly startDate,
            DateOnly? targetDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (startDate < today)
            {
                throw new BusinessException(
                    "開始日期不可早於今天。"
                );
            }

            if (targetDate.HasValue &&
                targetDate.Value <= startDate)
            {
                throw new BusinessException(
                    "目標日期必須晚於開始日期。"
                );
            }
        }
    }
}