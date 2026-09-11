namespace HappyEat.API.DTOs
{
    public class FoodImageCreateDto
    {
        public int UserId { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
    public class FoodImageResponseDto
    {
        public int ImageId { get; set; }
        public int UserId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime UploadTime { get; set; }

    }
}
