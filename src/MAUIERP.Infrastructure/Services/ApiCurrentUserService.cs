using MAUIERP.ApplicationLayer.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MAUIERP.Infrastructure.Services
{
    public class ApiCurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiCurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId => GetClaimValue("userId") ?? GetClaimValue(ClaimTypes.NameIdentifier);

        public string? UserName => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value
                                ?? _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;

        public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public IList<string> Roles => _httpContextAccessor.HttpContext?.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        public bool HasPermission(string permissionCode)
        {
            var permissions = _httpContextAccessor.HttpContext?.User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value);
            return permissions?.Contains(permissionCode) ?? false;
        }

        private Guid? GetClaimValue(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(claimType);
            return claim != null && Guid.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}
