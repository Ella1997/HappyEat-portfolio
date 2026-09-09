using HappyEat.API.DTOs;
using HappyEat.API.Models;

namespace HappyEat.API.Mappings
{
    public static class FoodImageMapping
    {
        //DTO->Entity
        public static FoodImage ToEntity(this FoodImageCreateDto dto, string imagePath)
        {
            return new FoodImage
            {
                UserId = dto.UserId,
                ImagePath = imagePath,
                UploadTime = DateTime.Now
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
