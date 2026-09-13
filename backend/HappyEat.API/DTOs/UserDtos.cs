namespace HappyEat.API.DTOs
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public DateOnly? BirthDate { get; set; }
        public string? Gender { get; set; }
    }
}
