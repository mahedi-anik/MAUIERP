using MAUIERP.Domain.Entities.Auth;
using MAUIERP.Domain.Entities.MasterData;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<Branch> Branches { get; }
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
