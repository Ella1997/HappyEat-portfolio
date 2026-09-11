using HappyEat.API.DTOs;
using HappyEat.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HappyEat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BodyRecordController:ControllerBase
    {
        private readonly IBodyRecordService _bodyRecordService;
        public BodyRecordController(IBodyRecordService bodyRecordService)
        {
            _bodyRecordService = bodyRecordService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BodyRecordResponseDto>>> GetByUserId(int userId)
        {
            var records = await _bodyRecordService.GetByUserIdAsync(userId);
            return Ok(records);
        }

        [HttpGet("{bodyRecordId}")]
        public async Task<ActionResult<BodyRecordResponseDto>> GetById(int bodyRecordId, int userId)
        {
            var record = await _bodyRecordService.GetByIdAsync(bodyRecordId, userId);
            return Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult<BodyRecordResponseDto>> Create(BodyRecordCreateDto dto)
        {
            var record = await _bodyRecordService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new
            {
                bodyRecordId = record.BodyRecordId,
                userId = record.UserId,
            }, record);
        }

        [HttpPut("{bodyRecordId}")]
        public async Task<IActionResult> Update(int bodyRecordId,int userId, BodyRecordUpdateDto dto)
        {
            await _bodyRecordService.UpdateAsync(bodyRecordId, userId, dto);
            return NoContent();
        }

        [HttpDelete("{bodyRecordId}")]
        public async Task<IActionResult> Delete(int bodyRecordId, int userId)
        {
            await _bodyRecordService.DeleteAsync(bodyRecordId, userId);
            return NoContent();
        }
    }
}
