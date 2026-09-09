namespace HappyEat.API.DTOs
{
    public class FoodImageCreateDto
    {
        public int UserId { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
}
