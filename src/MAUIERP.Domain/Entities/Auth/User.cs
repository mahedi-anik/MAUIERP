using MAUIERP.Domain.Common;
using MAUIERP.Domain.Enums;

namespace MAUIERP.Domain.Entities.Auth
{
    public class User : BaseAuditableEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
