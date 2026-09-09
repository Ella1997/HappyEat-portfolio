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
        //注入DB及環境資訊
        private readonly HappyEatDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        public FoodImageService(HappyEatDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }
        
        public async Task<FoodImageResponseDto> CreateAsync(FoodImageCreateDto dto)
        {
            //驗證user是否存在(不存在屬於業務錯誤)
            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == dto.UserId);

            if (!userExists) throw new NotFoundException("User Not Found.");

            //確認有收到圖片
            if (dto.Image == null || dto.Image.Length == 0) throw new BusinessException("Please upload an image.");

            //儲存圖片到wwwrooot/uploads
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads"); //取得wwwroot/uploads路徑
            Directory.CreateDirectory(uploadFolder);
            var extension = Path.GetExtension(dto.Image.FileName);//取得原始圖片副檔名
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadFolder, fileName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            //儲存圖片相對路徑到DB
            var imagePath = $"/uploads/{fileName}";
            var entity = dto.ToEntity(imagePath);
            _dbContext.FoodImages.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.ToResponseDto();
        }


        public async Task<FoodImageResponseDto?> GetByIdAsync(int imageId)
        {
            var entity = await _dbContext.FoodImages.FirstOrDefaultAsync(i => i.ImageId == imageId);
            if (entity == null) throw new NotFoundException("Food Image Not Found.");
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

            //3. 刪除圖片(wwwroot的實體跟DB的都要刪)
            if (!string.IsNullOrEmpty(entity.ImagePath))
            {
                var fileName = Path.GetFileName(entity.ImagePath);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                if(File.Exists(filePath)) File.Delete(filePath);
            }
            _dbContext.FoodImages.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
