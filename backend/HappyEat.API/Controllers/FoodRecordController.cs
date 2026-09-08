using HappyEat.API.DTOs;
using HappyEat.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HappyEat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodRecordController:ControllerBase
    {
        private readonly IFoodRecordService _foodRecordService;
        public FoodRecordController(IFoodRecordService foodRecordService)
        {
            _foodRecordService = foodRecordService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodRecordResponseDto>>> GetAll(int userId)
        {
            var records = await _foodRecordService.GetAllAsync(userId);
            return Ok(records);
        }

        [HttpGet("{recordId}")]
        public async Task<ActionResult<FoodImageResponseDto>> GetById(int userId, int recordId)
        {
            var record = await _foodRecordService.GetByIdAsync(userId, recordId);
            return Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult<FoodImageResponseDto>> Create(FoodRecordCreateDto dto)
        {
            var record = await _foodRecordService.CreateAsync(dto);
            return Ok(record);
        }

        [HttpPut("{recordId}")]
        public async Task<IActionResult> Update(int userId, int recordId, FoodRecordCreateDto dto)
        {
            await _foodRecordService.UpdateAsync(userId, recordId, dto);
            return NoContent();
        }

        [HttpDelete("{recordId}")]
        public async Task<IActionResult> Delete(int userId, int recordId)
        {
            await _foodRecordService.DeleteAsync(userId, recordId);
            return NoContent();
        }
    }
}
