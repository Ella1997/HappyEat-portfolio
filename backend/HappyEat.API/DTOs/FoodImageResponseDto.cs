namespace HappyEat.API.DTOs
{
    public class FoodImageResponseDto
    {
        public int ImageId { get; set; }
        public int UserId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime UploadTime {  get; set; }

    }
}
