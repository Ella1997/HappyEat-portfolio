using HappyEat.API.DTOs;

namespace HappyEat.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetByIdAsync(int userId);
    }
}
