using HappyEat.API.DTOs;

namespace HappyEat.API.Services.Interfaces
{
    public interface IBodyRecordService
    {
        Task<List<BodyRecordResponseDto>> GetByUserIdAsync(int userId);
        Task<BodyRecordResponseDto> GetByIdAsync(int bodyRecordId, int userId);
        Task<BodyRecordResponseDto> CreateAsync(BodyRecordCreateDto dto);
        Task UpdateAsync(int bodyRecordId, int userId, BodyRecordUpdateDto dto);
        Task DeleteAsync(int bodyRecordId, int userId);

    }
}
