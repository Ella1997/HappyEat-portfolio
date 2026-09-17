using HappyEat.API.DTOs;
using HappyEat.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HappyEat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodSearchController:ControllerBase
    {
        private readonly IFoodSearchService _foodSearchService;
        public FoodSearchController(IFoodSearchService foodSearchService)
        {
            _foodSearchService = foodSearchService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FoodSearchResponseDto>>> Search([FromQuery] string keyword)
        {
            var result = await _foodSearchService.SearchAsync(keyword);
            return Ok(result);
        }
    }
}
