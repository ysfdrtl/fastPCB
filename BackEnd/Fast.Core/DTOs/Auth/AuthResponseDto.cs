namespace Fast.Core.Dtos.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
