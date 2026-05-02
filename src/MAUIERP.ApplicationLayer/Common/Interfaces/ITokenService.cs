using MAUIERP.Domain.Entities.Auth;

namespace MAUIERP.ApplicationLayer.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
}
