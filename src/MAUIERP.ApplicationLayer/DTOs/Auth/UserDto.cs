namespace MAUIERP.ApplicationLayer.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Status { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
