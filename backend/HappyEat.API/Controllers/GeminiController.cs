using HappyEat.API.DTOs;
using HappyEat.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HappyEat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController:ControllerBase
    {
        private readonly IGeminiService _geminiService;
        public GeminiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("analyze/{imageId}")]
        public async Task<ActionResult<GeminiResponseDto>> Analyze(int imageId, [FromBody] string? userPrompt)
        {
            var result = await _geminiService.AnalyzeFoodImageAsync(imageId, userPrompt);

            return Ok(result);
        }
    }
}
