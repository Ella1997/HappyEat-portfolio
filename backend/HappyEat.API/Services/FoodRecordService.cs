using HappyEat.API.Data;
using HappyEat.API.DTOs;
using HappyEat.API.Mappings;
using HappyEat.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using HappyEat.API.Exceptions;
using HappyEat.API.Models;

namespace HappyEat.API.Services
{
    public class FoodRecordService : IFoodRecordService
    {
        private readonly HappyEatDbContext _dbContext;
        public FoodRecordService(HappyEatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<FoodRecordResponseDto> CreateAsync(FoodRecordCreateDto dto)
        {
            //驗證user
            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists) throw new NotFoundException("User Not Found.");

            //驗證image
            if (dto.ImageId.HasValue)
            {
                var image = await _dbContext.FoodImages.FirstOrDefaultAsync(i => i.ImageId == dto.ImageId);
                if (image == null) throw new NotFoundException("Image Not Found.");
                if (image.UserId != dto.UserId) throw new BusinessException("Image Not Belong to the User");
                var recordExists = await _dbContext.FoodRecords.AnyAsync(r => r.ImageId == dto.ImageId);
                if (recordExists) throw new ConflictException("The Image has been recorded.");
            }

            //驗證items
            foreach(var item in dto.Items)
            {
                if (item.FoodId.HasValue && item.DrinkId.HasValue) throw new BusinessException("An item cannot have both a FoodId and a DrinkId.");
                if (item.FoodId.HasValue)
                {
                    var foodExists = await _dbContext.Foods.AnyAsync(f => f.FoodId == item.FoodId);
                    if (!foodExists) throw new NotFoundException("Food Not Found.");
                }
                if (item.DrinkId.HasValue)
                {
                    var drinkExists = await _dbContext.Drinks.AnyAsync(d => d.DrinkId == item.DrinkId);
                    if (!drinkExists) throw new NotFoundException("Drink Not Found.");
                }
            }

            var entity = dto.ToEntity();
            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.ToResponseDto();
        }

        public async Task DeleteAsync(int userId, int recordId)
        {
            var record = await GetRecordAsync(userId, recordId, false);
            var imageId = record.ImageId;
            _dbContext.FoodRecords.Remove(record);
            if (imageId.HasValue)
            {
                var image = await _dbContext.FoodImages.FirstOrDefaultAsync(r => r.ImageId == imageId);
                if (image != null) _dbContext.FoodImages.Remove(image);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<FoodRecordResponseDto>> GetAllAsync(int userId)
        {
            var records = await _dbContext.FoodRecords
                .AsNoTracking()//唯獨查詢
                .Include(r=>r.FoodRecordItems)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RecordDate)
                .ToListAsync();

            return records.Select(r => r.ToResponseDto());
        }

        public async Task<FoodRecordResponseDto> GetByIdAsync(int userId, int recordId)
        {
            var record = await GetRecordAsync(userId, recordId,true);
            return record.ToResponseDto();
        }


        public async Task UpdateAsync(int userId, int recordId, FoodRecordCreateDto dto)
        {
            var record = await GetRecordAsync(userId, recordId, true);
            var oldImageId = record.ImageId;

            record.ImageId = dto.ImageId;
            record.RecordSource = dto.RecordSource;
            record.MealType = dto.MealType;
            record.Description = dto.Description;

            _dbContext.FoodRecordItems.RemoveRange(record.FoodRecordItems);

            record.FoodRecordItems = dto.Items.Select
                (item => new FoodRecordItem
                {
                    ItemName = item.ItemName,
                    FoodId = item.FoodId,
                    DrinkId = item.DrinkId,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Calories = item.Calories,
                    Carbs = item.Carbs,
                    Protein = item.Protein,
                    Fat = item.Fat
                }).ToList();
            
            if(oldImageId.HasValue && oldImageId != dto.ImageId)
            {
                var oldImage = await _dbContext.FoodImages.FirstOrDefaultAsync(i => i.ImageId == oldImageId);
                if (oldImage != null) _dbContext.FoodImages.Remove(oldImage);
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<FoodRecord> GetRecordAsync(int userId, int recordId, bool includeItems = false)
        {
            IQueryable<FoodRecord> query = _dbContext.FoodRecords;

            if (includeItems) query = query.Include(r => r.FoodRecordItems);

            var record = await query.FirstOrDefaultAsync(r => r.UserId == userId && r.RecordId == recordId);

            if (record == null) throw new NotFoundException("Record Not Found.");

            return record;
        }

    }
}
