using HappyEat.API.Data;
using HappyEat.API.DTOs;
using HappyEat.API.Exceptions;
using HappyEat.API.Mappings;
using HappyEat.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyEat.API.Services
{
    public class BodyRecordService:IBodyRecordService
    {
        private readonly HappyEatDbContext _dbContext;
        public BodyRecordService(HappyEatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BodyRecordResponseDto> CreateAsync(BodyRecordCreateDto dto)
        {
            var newRecord = dto.ToEntity();
            _dbContext.BodyRecords.Add(newRecord);
            await _dbContext.SaveChangesAsync();
            return newRecord.ToResponseDto();
        }

        public async Task DeleteAsync(int bodyRecordId, int userId)
        {
            var record = await _dbContext.BodyRecords
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BodyRecordId == bodyRecordId);
            if (record == null) throw new NotFoundException("Record Not Found.");

            _dbContext.BodyRecords.Remove(record);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BodyRecordResponseDto> GetByIdAsync(int bodyRecordId, int userId)
        {
            var record = await _dbContext.BodyRecords.AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BodyRecordId == bodyRecordId);
            if (record == null) throw new NotFoundException("Record Not Found.");

            return record.ToResponseDto();
        }

        public async Task<List<BodyRecordResponseDto>> GetByUserIdAsync(int userId)
        {
            var records = await _dbContext.BodyRecords.AsNoTracking()
                .Where(r => r.UserId == userId).OrderByDescending(r => r.RecordDate).ToListAsync();

            return records.Select(r => r.ToResponseDto()).ToList();
        }

        public async Task UpdateAsync(int bodyRecordId, int userId, BodyRecordUpdateDto dto)
        {
            var record = await _dbContext.BodyRecords
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BodyRecordId == bodyRecordId);
            if (record == null) throw new NotFoundException("Record Not Found.");

            record.RecordDate = dto.RecordDate;
            record.ActivityLevel = dto.ActivityLevel;
            record.Height = dto.Height;
            record.Weight = dto.Weight;
            record.WaistSize = dto.WaistSize;
            record.NeckSize = dto.NeckSize;
            record.HipSize = dto.HipSize;
            record.BodyFat = dto.BodyFat;
            record.MuscleMass = dto.MuscleMass;
            record.VisceralFat = dto.VisceralFat;

            //_dbContext.Update(record);因為EF Core已自動追蹤有修改過的資料, 所以update不用特別寫
            await _dbContext.SaveChangesAsync();

        }
    }
}
