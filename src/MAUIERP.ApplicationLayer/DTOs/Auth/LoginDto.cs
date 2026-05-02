namespace MAUIERP.ApplicationLayer.DTOs.Auth
{
    public class LoginDto
    {
        public required string UsernameOrEmail { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
