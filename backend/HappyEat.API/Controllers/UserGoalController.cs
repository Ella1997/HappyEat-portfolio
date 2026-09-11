using HappyEat.API.DTOs;
using HappyEat.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HappyEat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserGoalController:ControllerBase
    {
        private readonly IUserGoalService _userGoalService;
        public UserGoalController(IUserGoalService userGoalService)
        {
            _userGoalService = userGoalService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserGoalResponseDto>>> GetByUserId(int userId)
        {
            var goals = await _userGoalService.GetByUserIdAsync(userId);
            return Ok(goals);
        }

        [HttpGet("goalId")]
        public async Task<ActionResult<UserGoalResponseDto>> GetById(int userId, int goalId)
        {
            var goal = await _userGoalService.GetByIdAsync(userId, goalId);
            return Ok(goal);
        }

        [HttpPost]
        public async Task<ActionResult<UserGoalResponseDto>> Create(UserGoalCreateDto dto)
        {
            var goal = await _userGoalService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new
            {
                goalId = goal.GoalId,
                userId = goal.UserId
            }, goal);
        }

        [HttpPut("{goalId}")]
        public async Task<IActionResult> Update(int goalId, int userId, UserGoalUpdateDto dto)
        {
            await _userGoalService.UpdateAsync(goalId, userId, dto);
            return NoContent();
        }

        [HttpDelete("{goalId}")]
        public async Task<IActionResult> Delete(int goalId, int userId)
        {
            await _userGoalService.DeleteAsync(goalId, userId);
            return NoContent();
        }
    }
}
