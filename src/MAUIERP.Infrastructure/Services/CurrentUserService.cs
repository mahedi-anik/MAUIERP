using MAUIERP.ApplicationLayer.Common.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace MAUIERP.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        // Instead of HttpContext, we hold the ClaimsPrincipal in memory
        private ClaimsPrincipal? _user;

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _user?.FindFirst("userId") ??
                                  _user?.FindFirst(ClaimTypes.NameIdentifier);

                return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
            }
        }

        public string? UserName => _user?.FindFirst(ClaimTypes.Name)?.Value ??
                                   _user?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;

        public string? Email => _user?.FindFirst(ClaimTypes.Email)?.Value ??
                                _user?.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;

        public IList<string> Roles => _user?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        // New method to populate the user from your Auth process
        public void SetUser(ClaimsPrincipal principal)
        {
            _user = principal;
        }

        public bool HasPermission(string permissionCode)
        {
            return true; // Simplified for now
        }
    }
}