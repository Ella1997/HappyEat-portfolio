namespace HappyEat.API.DTOs
{
    public class FoodRecordCreateDto
    {
        public int UserId {  get; set; }
        public int? ImageId {  get; set; }
        public string RecordSource { get; set; } = null!;
        public DateTime RecordDate { get; set; }
        public string? MealType { get; set; }
        public string? Description {  get; set; }
        public List<FoodRecordItemCreateDto> Items { get; set; } = new();

    }
}
