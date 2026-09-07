using HappyEat.API.DTOs;
using HappyEat.API.Models;

namespace HappyEat.API.Mappings
{
    public static class FoodDetectedMapping
    {
        //DTO->Entity
        public static FoodDetected ToEntity(this FoodDetectedCreateDto dto)
        {
            return new FoodDetected
            {
                ImageId = dto.ImageId,
                EstimatedFood = dto.EstimatedFood,
                EstimatedWeight = dto.EstimatedWeight,
                EstimatedCalories = dto.EstimatedCalories,
                EstimatedCarbs = dto.EstimatedCarbs,
                EstimatedProtein = dto.EstimatedProtein,
                EstimatedFat = dto.EstimatedFat,
                Ainote = dto.Ainote,
            };
        }

        //Entity->Response DTO
        public static FoodDetectedResponseDto ToResponse(this FoodDetected entity)
        {
            return new FoodDetectedResponseDto
            {
                DetectId = entity.DetectId,
                EstimatedFood = entity.EstimatedFood,
                EstimatedWeight = entity.EstimatedWeight,
                EstimatedCalories = entity.EstimatedCalories,
                EstimatedCarbs = entity.EstimatedCarbs,
                EstimatedProtein = entity.EstimatedProtein,
                EstimatedFat = entity.EstimatedFat,
                Ainote = entity.Ainote,
            };
        }
    }
}
