using HappyEat.API.DTOs;
using HappyEat.API.Models;

namespace HappyEat.API.Mappings
{
    public static class BodyRecordMapping
    {
        //Entity->Response DTO
        public static BodyRecordResponseDto ToResponseDto(this BodyRecord record)
        {
            return new BodyRecordResponseDto
            {
                BodyRecordId = record.BodyRecordId,
                UserId = record.UserId,
                RecordDate = record.RecordDate,
                ActivityLevel = record.ActivityLevel,
                Height = record.Height,
                Weight = record.Weight,
                WaistSize = record.WaistSize,
                NeckSize = record.NeckSize,
                HipSize = record.HipSize,
                BodyFat = record.BodyFat,
                MuscleMass = record.MuscleMass,
                VisceralFat = record.VisceralFat,
            };
        }

        //DTO->Entity
        public static BodyRecord ToEntity(this BodyRecordCreateDto dto)
        {
            return new BodyRecord
            {
                UserId = dto.UserId,
                RecordDate = dto.RecordDate,
                ActivityLevel = dto.ActivityLevel,
                Height = dto.Height,
                Weight = dto.Weight,
                WaistSize = dto.WaistSize,
                NeckSize = dto.NeckSize,
                HipSize = dto.HipSize,
                BodyFat = dto.BodyFat,
                MuscleMass = dto.MuscleMass,
                VisceralFat = dto.VisceralFat
            };
        }
    }
}
