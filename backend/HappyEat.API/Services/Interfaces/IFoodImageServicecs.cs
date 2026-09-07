using HappyEat.API.DTOs;

namespace HappyEat.API.Services.Interfaces
{
    public interface IFoodImageServicecs
    {
        Task<FoodImageResponseDto> CreateAsync(FoodImageCreateDto dto);
        Task<FoodImageResponseDto?> GetByIdAsync(int imageId);
        Task<List<FoodImageResponseDto>> GetByUserIdAsync(int userId);
        Task DeleteAsync(int imageId); //不需要回傳bool, 因為這邊設計是成功就正常刪除, 找不到圖片就報exception, 有關聯狀況存在則不允許刪除
    }
}
