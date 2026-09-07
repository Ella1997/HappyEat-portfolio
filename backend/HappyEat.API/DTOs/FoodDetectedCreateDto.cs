namespace HappyEat.API.DTOs
{
    public class FoodDetectedCreateDto
    {
        public int ImageId { get; set; }
        public string? EstimatedFood { get; set; }
        public decimal? EstimatedWeight { get; set; }
        public decimal EstimatedCalories { get; set; }
        public decimal? EstimatedCarbs {  get; set; }
        public decimal? EstimatedProtein {  get; set; }
        public decimal? EstimatedFat { get; set; }
        public string? Ainote {  get; set; }
    }
}
