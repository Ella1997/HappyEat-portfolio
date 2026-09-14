using HappyEat.API.DTOs;

namespace HappyEat.API.Services.Interfaces
{
    public interface IUserGoalService
    {
        Task<List<UserGoalResponseDto>> GetByUserIdAsync(int userId);
        Task<UserGoalResponseDto> GetByIdAsync(int userId, int goalId);
        Task<UserGoalResponseDto> CreateAsync(UserGoalCreateDto dto);
        Task UpdateAsync(int userId, int goalId, UserGoalUpdateDto dto);
        Task EndGoalAsync(int userId, int goalId);

    }
}
