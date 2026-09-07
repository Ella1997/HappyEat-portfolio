using HappyEat.API.DTOs;
using HappyEat.API.Models;

namespace HappyEat.API.Mappings
{
    public static class FoodImageMapping
    {
        //DTO->Entity
        public static FoodImage ToEntity(this FoodImageCreateDto dto)
        {
            return new FoodImage
            {
                UserId = dto.UserId,
                ImagePath = dto.ImagePath,
            };
        }

        //Entity->Response DTO
        public static FoodImageResponseDto ToResponseDto(this FoodImage entity)
        {
            return new FoodImageResponseDto
            {
                ImageId = entity.ImageId,
                UserId = entity.UserId,
                ImagePath = entity.ImagePath,
                UploadTime = entity.UploadTime
            };
        }
    }
}
