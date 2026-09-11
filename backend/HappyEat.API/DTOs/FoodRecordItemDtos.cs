namespace HappyEat.API.DTOs
{
    public class FoodRecordItemCreateDto
    {
        public string? ItemName { get; set; }
        public int? FoodId { get; set; }
        public int? DrinkId {  get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal Calories { get; set; }
        public decimal? Carbs {  get; set; }
        public decimal? Protein { get; set; }
        public decimal? Fat { get; set; }
    }
    public class FoodRecordItemResponseDto
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int RecordId { get; set; }
        public int? FoodId { get; set; }
        public int? DrinkId { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal? Calories { get; set; }
        public decimal? Carbs { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Fat { get; set; }
    }
}
