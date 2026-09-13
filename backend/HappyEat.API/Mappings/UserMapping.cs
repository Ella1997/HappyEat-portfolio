using HappyEat.API.DTOs;
using HappyEat.API.Models;

namespace HappyEat.API.Mappings
{
    public static class UserMapping
    {
        public static UserResponseDto ToResponseDto(this User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
            };
        }
    }
}
