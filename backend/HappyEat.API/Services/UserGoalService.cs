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
            var today = DateOnly.FromDateTime(DateTime.Today);

            //確認目標開始日有體態紀錄
            var hasBodyRecord = await _dbContext.BodyRecords
                .AnyAsync(r => r.UserId == dto.UserId && r.RecordDate == today);

            if (!hasBodyRecord) throw new BusinessException("建立目標前，請先新增開始日當天的體態紀錄。");
            
            //建立新目標前將舊目標停用
            var activeGoals = await _dbContext.UserGoals.Where(g => g.UserId == dto.UserId && g.IsActive).ToListAsync();
            foreach(var goal in activeGoals)
            {
                goal.IsActive = false;
            }

            //驗證目標日期
            ValidateTargetDate(dto.TargetDate);

            var newGoal = dto.ToEntity();
            _dbContext.UserGoals.Add(newGoal);
            await _dbContext.SaveChangesAsync();
            return newGoal.ToResponseDto();
        }

        public async Task EndGoalAsync(int userId, int goalId)
        {
            var goal = await _dbContext.UserGoals
                .FirstOrDefaultAsync(g =>g.GoalId == goalId && g.UserId == userId && g.IsActive);

            if (goal == null)
            {
                throw new NotFoundException("找不到進行中的目標。");
            }

            goal.IsActive = false;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserGoalResponseDto> GetByIdAsync(int userId, int goalId)
        {
            var goal = await _dbContext.UserGoals.AsNoTracking()
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GoalId == goalId);

            if (goal == null) throw new NotFoundException("找不到此目標");

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
            var goal = await _dbContext.UserGoals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GoalId == goalId && g.IsActive);

            if (goal == null) throw new NotFoundException("找不到進行中的目標");

            //驗證目標日期
            ValidateTargetDate(dto.TargetDate);

            goal.GoalType = dto.GoalType;
            goal.TargetWeight = dto.TargetWeight;
            goal.TargetBodyFat = dto.TargetBodyFat;
            goal.TargetDate = dto.TargetDate;

            await _dbContext.SaveChangesAsync();
        }

        private static void ValidateTargetDate(DateOnly? targetDate)
        {
            if (targetDate.HasValue &&
                targetDate.Value <= DateOnly.FromDateTime(DateTime.Today))
            {
                throw new BusinessException(
                    "目標日期必須晚於今天。"
                );
            }
        }
    }
}
