using System.Globalization;

namespace HappyEat.API.DTOs
{
    public class FoodSearchResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Unit { get; set; }
        public decimal Calories { get; set; }
        public decimal? Carbs { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Fat { get; set; }
    }
}
