using HappyEat.API.Data;
using HappyEat.API.DTOs;
using HappyEat.API.Mappings;
using HappyEat.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using HappyEat.API.Exceptions;

namespace HappyEat.API.Services
{
    public class FoodImageService : IFoodImageService
    {
        //注入DB
        private readonly HappyEatDbContext _dbContext;
        public FoodImageService(HappyEatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<FoodImageResponseDto> CreateAsync(FoodImageCreateDto dto)
        {
            //先驗證user是否存在(不存在屬於業務錯誤)
            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == dto.UserId);

            if (!userExists) throw new NotFoundException("User Not Found.");

            var entity = dto.ToEntity();
            _dbContext.FoodImages.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.ToResponseDto();
        }


        public async Task<FoodImageResponseDto?> GetByIdAsync(int imageId)
        {
            var entity = await _dbContext.FoodImages.FirstOrDefaultAsync(i => i.ImageId == imageId);
            if (entity == null) return null;
            return entity.ToResponseDto();
        }

        public async Task<List<FoodImageResponseDto>> GetByUserIdAsync(int userId)
        {
            var userExists = await _dbContext.Users
                .AnyAsync(u => u.UserId == userId);

            if (!userExists)
            {
                throw new NotFoundException("User not found.");
            }

            var entities = await _dbContext.FoodImages
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.UploadTime)
                .ToListAsync();
            return entities.Select(i => i.ToResponseDto()).ToList();
        }

        public async Task DeleteAsync(int imageId)
        {
            //1. 找圖片
            var entity = await _dbContext.FoodImages.FirstOrDefaultAsync(i => i.ImageId == imageId);
            if (entity == null) throw new NotFoundException("Food Image Not Found.");

            //2. 確認圖片是否被FoodRecord使用
            var isUsed = await _dbContext.FoodRecords.AnyAsync(r => r.ImageId == imageId);
            if (isUsed) throw new ConflictException("Food image is already used by a food record and cannot be deleted.");

            //3. 刪除圖片
            _dbContext.FoodImages.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
