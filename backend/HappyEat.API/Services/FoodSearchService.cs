using HappyEat.API.Data;
using HappyEat.API.DTOs;
using HappyEat.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyEat.API.Services
{
    public class FoodSearchService : IFoodSearchService
    {
        private readonly HappyEatDbContext _dbContext;
        public FoodSearchService(HappyEatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<FoodSearchResponseDto>> SearchAsync(string keyword)
        {
            // 1. 關鍵字為空時直接回傳空結果
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<FoodSearchResponseDto>();
            }

            keyword = keyword.Trim();

            // 2. 搜尋 Food
            // Food 資料庫中的營養資訊皆以 100g 為基準
            var foods = await _dbContext.Foods
                .AsNoTracking()
                .Where(f => f.FoodName.Contains(keyword))
                .Select(f => new FoodSearchResponseDto
                {
                    Id = f.FoodId,
                    Type = "Food",
                    Name = f.FoodName,
                    Unit = "100g",
                    Calories = f.Calories,
                    Carbs = f.Carbs,
                    Protein = f.Protein,
                    Fat = f.Fat
                })
                .Take(10)
                .ToListAsync();

            // 3. 搜尋 Drink
            // Drink 的 Size：
            // M -> 500ml
            // L -> 700ml
            var drinks = await _dbContext.Drinks
                .AsNoTracking()
                .Where(d =>
                    d.DrinkName != null &&
                    d.DrinkName.Contains(keyword))
                .Select(d => new FoodSearchResponseDto
                {
                    Id = d.DrinkId,
                    Type = "Drink",
                    Name = d.DrinkName!,

                    Unit = d.Size == "M"
                        ? "500ml"
                        : d.Size == "L"
                            ? "700ml"
                            : d.Size ?? "份",

                    Calories = d.Calories ?? 0,
                    Carbs = d.Carbs,
                    Protein = d.Protein,
                    Fat = d.Fat
                })
                .Take(10)
                .ToListAsync();

            // 4. 合併 Food + Drink 搜尋結果
            return foods
                .Concat(drinks)
                .Take(10)
                .ToList();
        }
    }
}
