using HappyEat.API.DTOs;

namespace HappyEat.API.Services.Interfaces
{
    public interface IFoodRecordService
    {
        Task<IEnumerable<FoodRecordResponseDto>> GetAllAsync(int userId); //取得某使用者所有飲食紀錄, 若無則回傳[]->不需要BusinessException
        Task<FoodRecordResponseDto> GetByIdAsync(int userId, int recordId); //找不到某筆飲食紀錄或使用者->BusinessException處理(底下update&delete同理)
        Task<FoodRecordResponseDto> CreateAsync(FoodRecordCreateDto dto);
        Task UpdateAsync(int userId, int recordId, FoodRecordCreateDto dto);
        Task DeleteAsync(int userId, int recordId);
    }
}
