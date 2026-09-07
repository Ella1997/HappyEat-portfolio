using HappyEat.API.DTOs;
using HappyEat.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace HappyEat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodImageController:ControllerBase
    {
        //注入FoodImageService
        private readonly IFoodImageServicecs _service;
        public FoodImageController(IFoodImageServicecs service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<FoodImageResponseDto>> Create(FoodImageCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);

            return Ok(result);
        }

        [HttpGet("{imageId}")]
        public async Task<ActionResult<FoodImageResponseDto>> GetById(int imageId)
        {
            var result = await _service.GetByIdAsync(imageId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<FoodImageResponseDto>>> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            await _service.DeleteAsync(imageId);
            return NoContent();
        }
    }
}
