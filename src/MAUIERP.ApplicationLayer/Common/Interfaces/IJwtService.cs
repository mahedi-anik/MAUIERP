using MAUIERP.Domain.Entities.Auth;
using System.Security.Claims;

namespace MAUIERP.ApplicationLayer.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, IList<string> roles, IList<Claim> additionalClaims = null);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
