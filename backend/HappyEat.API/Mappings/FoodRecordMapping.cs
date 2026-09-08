using HappyEat.API.DTOs;
using HappyEat.API.Models;

namespace HappyEat.API.Mappings
{
    public static class FoodRecordMapping
    {
        //DTO->Entity
        public static FoodRecord ToEntity(this FoodRecordCreateDto dto)
        {
            return new FoodRecord
            {
                UserId = dto.UserId,
                ImageId = dto.ImageId,
                RecordSource = dto.RecordSource,
                RecordDate = dto.RecordDate,
                MealType = dto.MealType,
                Description = dto.Description,
                FoodRecordItems = dto.Items.Select(item=>new FoodRecordItem
                {
                    ItemName = item.ItemName,
                    FoodId = item.FoodId,
                    DrinkId = item.DrinkId,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Calories = item.Calories,
                    Carbs = item.Carbs,
                    Protein = item.Protein,
                    Fat = item.Fat,
                }).ToList()
            };
        }
    
        //Entity->Response DTO
        public static FoodRecordResponseDto ToResponseDto(this FoodRecord entity)
        {
            return new FoodRecordResponseDto
            {
                RecordId = entity.RecordId,
                UserId = entity.UserId,
                ImageId = entity.ImageId,
                RecordSource = entity.RecordSource,
                RecordDate = entity.RecordDate,
                MealType = entity.MealType,
                Description = entity.Description,
                Items = entity.FoodRecordItems
                    .Select(item => new FoodRecordItemResponseDto
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        RecordId = item.RecordId,
                        FoodId = item.FoodId,
                        DrinkId = item.DrinkId,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Calories = item.Calories,
                        Carbs = item.Carbs,
                        Protein = item.Protein,
                        Fat = item.Fat
                    }).ToList()
            };
        }
    }
}
