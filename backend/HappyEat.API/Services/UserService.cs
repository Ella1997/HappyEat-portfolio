using HappyEat.API.Data;
using HappyEat.API.DTOs;
using HappyEat.API.Exceptions;
using HappyEat.API.Mappings;
using HappyEat.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyEat.API.Services
{
    public class UserService : IUserService
    {
        private readonly HappyEatDbContext _dbContext;
        public UserService(HappyEatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UserResponseDto> GetByIdAsync(int userId)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) throw new NotFoundException("User Not Found.");
            return user.ToResponseDto();
        }
    }
}
