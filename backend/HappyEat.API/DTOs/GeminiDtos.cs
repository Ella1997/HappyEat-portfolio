namespace HappyEat.API.DTOs
{
    public class GeminiResponseDto
    {
        public int ImageId { get; set; }

        //Total由後端計算, 不是由Gemini直接提供
        public decimal TotalCalories { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalFat { get; set; }
        public List<GeminiFoodItemDto> Items { get; set; } = new();
        public string Description { get; set; } = string.Empty; //AI note
    }

    public class GeminiFoodItemDto 
    {
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; } = 1;
        public string Unit { get; set; } = "份";
        public decimal Calories { get; set; }
        public decimal Carbs {  get; set; }
        public decimal Protein { get; set; }
        public decimal Fat { get; set; }
    }
}
