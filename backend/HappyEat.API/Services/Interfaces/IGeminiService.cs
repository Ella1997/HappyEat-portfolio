using HappyEat.API.DTOs;

namespace HappyEat.API.Services.Interfaces
{
    public interface IGeminiService
    {
        Task<GeminiResponseDto> AnalyzeFoodImageAsync(int imageId, string? userPrompt = null); //userPrompt是讓使用者決定是否用文字補充說明然後請Gemini重新辨識
    }
}
