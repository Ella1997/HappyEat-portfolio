using HappyEat.API.DTOs;

namespace HappyEat.API.Services.Interfaces
{
    public interface IFoodSearchService
    {
        Task<List<FoodSearchResponseDto>> SearchAsync(string keyword);
    }
}
